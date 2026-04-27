// Admin UI: list/create tenants and configure SAML per tenant.
// Protected by HTTP Basic Auth (requireAdmin middleware).

import { Router } from 'express';
import multer from 'multer';
import { tenants, samlConfigs, attrMappings } from '../db.js';
import { parseIdpMetadata } from '../utils/metadataParser.js';
import { parseCert } from '../utils/certParser.js';
import { PRESETS, CANONICAL_FIELDS } from '../utils/presets.js';
import { spEntityId, acsUrl } from '../auth/samlStrategyResolver.js';
import { config } from '../config.js';

const upload = multer({ storage: multer.memoryStorage(), limits: { fileSize: 1024 * 1024 } });

export function adminRouter() {
  const r = Router();

  r.get('/', (req, res) => {
    res.render('admin/index', { tenants: tenants.list() });
  });

  r.get('/tenants/new', (req, res) => {
    res.render('admin/tenant-new', { error: null, values: {} });
  });

  r.post('/tenants', (req, res) => {
    const slug = (req.body.slug || '').toLowerCase().trim();
    const name = (req.body.name || '').trim();
    const email_domain = (req.body.email_domain || '').toLowerCase().trim() || null;

    if (!/^[a-z0-9][a-z0-9-]{1,40}$/.test(slug) || !name) {
      return res.status(400).render('admin/tenant-new', {
        error: 'Slug must be lowercase letters/digits/hyphens (2-41 chars); name required.',
        values: { slug, name, email_domain },
      });
    }

    try {
      tenants.create({ slug, name, email_domain });
    } catch (err) {
      return res.status(400).render('admin/tenant-new', {
        error: err.message.includes('UNIQUE') ? 'Slug or email domain already in use.' : err.message,
        values: { slug, name, email_domain },
      });
    }
    res.redirect(`/admin/tenants/${slug}`);
  });

  r.post('/tenants/:slug/delete', (req, res) => {
    tenants.deleteBySlug(req.params.slug);
    res.redirect('/admin');
  });

  r.get('/tenants/:slug', (req, res) => {
    const tenant = tenants.getBySlug(req.params.slug);
    if (!tenant) return res.status(404).send('Unknown tenant');
    const cfg = samlConfigs.getByTenantId(tenant.id);
    let certInfo = null;
    if (cfg) {
      try { certInfo = parseCert(cfg.x509_cert); } catch { certInfo = null; }
    }
    res.render('admin/tenant-detail', {
      tenant,
      cfg,
      certInfo,
      mappings: attrMappings.getByTenantId(tenant.id),
      urls: {
        spEntityId: spEntityId(tenant.slug),
        acsUrl: acsUrl(tenant.slug),
        metadataUrl: `${config.appBaseUrl}/metadata/${tenant.slug}`,
      },
    });
  });

  r.get('/tenants/:slug/saml-config', (req, res) => {
    const tenant = tenants.getBySlug(req.params.slug);
    if (!tenant) return res.status(404).send('Unknown tenant');
    const cfg = samlConfigs.getByTenantId(tenant.id);
    const existingMap = Object.fromEntries(
      attrMappings.getByTenantId(tenant.id).map((m) => [m.canonical_field, m.source_claim]),
    );
    res.render('admin/saml-config', {
      tenant,
      cfg,
      existingMap,
      presets: PRESETS,
      canonicalFields: CANONICAL_FIELDS,
      error: null,
    });
  });

  // One endpoint, two modes: metadata file upload OR manual fields. Cert upload is a separate file.
  r.post(
    '/tenants/:slug/saml-config',
    upload.fields([{ name: 'metadataFile', maxCount: 1 }, { name: 'certFile', maxCount: 1 }]),
    (req, res, next) => {
      const tenant = tenants.getBySlug(req.params.slug);
      if (!tenant) return res.status(404).send('Unknown tenant');

      const renderError = (msg) => res.status(400).render('admin/saml-config', {
        tenant,
        cfg: samlConfigs.getByTenantId(tenant.id),
        existingMap: Object.fromEntries(
          attrMappings.getByTenantId(tenant.id).map((m) => [m.canonical_field, m.source_claim]),
        ),
        presets: PRESETS,
        canonicalFields: CANONICAL_FIELDS,
        error: msg,
      });

      try {
        let idp_entity_id, sso_url, slo_url, x509_cert, nameid_format;

        if (req.body.mode === 'metadata') {
          const file = req.files?.metadataFile?.[0];
          if (!file) return renderError('Metadata XML file is required in "Upload metadata" mode.');
          const parsed = parseIdpMetadata(file.buffer.toString('utf8'));
          idp_entity_id = parsed.idpEntityId;
          sso_url = parsed.ssoUrl;
          slo_url = parsed.sloUrl;
          x509_cert = parsed.x509Cert;
          nameid_format = parsed.nameIdFormat;
        } else {
          idp_entity_id = (req.body.idp_entity_id || '').trim();
          sso_url = (req.body.sso_url || '').trim();
          slo_url = (req.body.slo_url || '').trim() || null;
          nameid_format = req.body.nameid_format || 'urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress';

          const certFile = req.files?.certFile?.[0];
          const existing = samlConfigs.getByTenantId(tenant.id);
          if (certFile) {
            x509_cert = certFile.buffer.toString('utf8');
          } else if (existing) {
            x509_cert = existing.x509_cert;
          } else {
            return renderError('Signing certificate file is required.');
          }
          if (!idp_entity_id || !sso_url) {
            return renderError('IdP Entity ID and SSO URL are required.');
          }
        }

        // Validate and normalize (accepts PEM or bare base64, always stores PEM).
        x509_cert = parseCert(x509_cert).pem;

        samlConfigs.upsert({
          tenant_id: tenant.id,
          idp_entity_id,
          sso_url,
          slo_url,
          x509_cert,
          nameid_format,
        });

        // Collect attribute map: standard rows + optional custom rows.
        const standard = req.body.mapping || {};
        const customFields = [].concat(req.body.custom_field || []);
        const customClaims = [].concat(req.body.custom_claim || []);
        const mappings = [];
        for (const field of CANONICAL_FIELDS) {
          if (standard[field]) mappings.push({ canonical_field: field, source_claim: standard[field].trim() });
        }
        for (let i = 0; i < customFields.length; i++) {
          const f = (customFields[i] || '').trim();
          const c = (customClaims[i] || '').trim();
          if (f && c) mappings.push({ canonical_field: f, source_claim: c });
        }
        attrMappings.replaceAll(tenant.id, mappings);

        res.redirect(`/admin/tenants/${tenant.slug}`);
      } catch (err) {
        next(err);
      }
    },
  );

  return r;
}
