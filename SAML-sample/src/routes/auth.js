// Login + SAML endpoints.
// Flow:
//   GET  /login                -> email entry form
//   POST /login                -> discover tenant by email domain -> redirect to /login/:slug
//   GET  /login/:slug          -> passport initiates SAML (302 to IdP)
//   POST /callback/:slug       -> IdP POSTs the SAMLResponse here (ACS)
//   GET  /metadata/:slug       -> SP metadata XML (useful for IdP config)
//   POST /logout               -> local logout

import { Router } from 'express';
import passport from 'passport';
import { SAML } from '@node-saml/node-saml';
import { tenants, samlConfigs } from '../db.js';
import { resolveTenantByEmail } from '../auth/tenantResolver.js';
import { spEntityId, acsUrl } from '../auth/samlStrategyResolver.js';

export function authRouter() {
  const r = Router();

  r.get('/login', (req, res) => {
    res.render('login', { allTenants: tenants.list(), error: null });
  });

  r.post('/login', (req, res) => {
    const { email, slug } = req.body;
    let targetSlug = slug;
    if (!targetSlug && email) {
      const t = resolveTenantByEmail(email);
      if (t) targetSlug = t.slug;
    }
    if (!targetSlug) {
      return res.status(400).render('login', {
        allTenants: tenants.list(),
        error: 'No tenant matches that email domain. Pick a tenant below, or contact your admin.',
      });
    }
    res.redirect(`/login/${targetSlug}`);
  });

  r.get('/login/:slug', (req, res, next) => {
    const tenant = tenants.getBySlug(req.params.slug);
    if (!tenant) return res.status(404).render('login', { allTenants: tenants.list(), error: `Unknown tenant: ${req.params.slug}` });
    if (!samlConfigs.getByTenantId(tenant.id)) {
      return res.status(400).render('login', {
        allTenants: tenants.list(),
        error: `Tenant '${tenant.slug}' has no SAML configuration yet. Contact your admin.`,
      });
    }
    passport.authenticate('saml', { failureRedirect: '/login' })(req, res, next);
  });

  r.post(
    '/callback/:slug',
    (req, res, next) => {
      const tenant = tenants.getBySlug(req.params.slug);
      if (!tenant || !samlConfigs.getByTenantId(tenant.id)) {
        return res.status(400).render('login', {
          allTenants: tenants.list(),
          error: 'SAML callback received for an unknown or unconfigured tenant.',
        });
      }
      passport.authenticate('saml', { failureRedirect: '/login' })(req, res, next);
    },
    (req, res) => res.redirect('/app'),
  );

  r.get('/metadata/:slug', (req, res) => {
    const tenant = tenants.getBySlug(req.params.slug);
    if (!tenant) return res.status(404).send('Unknown tenant');
    const cfg = samlConfigs.getByTenantId(tenant.id);
    if (!cfg) return res.status(404).send('Tenant has no SAML configuration yet');

    const saml = new SAML({
      entryPoint: cfg.sso_url,
      issuer: spEntityId(req.params.slug),
      callbackUrl: acsUrl(req.params.slug),
      idpCert: cfg.x509_cert,
      identifierFormat: cfg.nameid_format,
      wantAssertionsSigned: true,
    });
    const xml = saml.generateServiceProviderMetadata(null);
    res.type('application/xml').send(xml);
  });

  r.post('/logout', (req, res, next) => {
    req.logout((err) => {
      if (err) return next(err);
      req.session.destroy(() => res.redirect('/login'));
    });
  });

  return r;
}
