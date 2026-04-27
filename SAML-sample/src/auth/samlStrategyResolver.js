// Builds the MultiSamlStrategy. For every incoming /login/:slug and /callback/:slug
// request, getSamlOptions is called to load that tenant's IdP config from SQLite.
// This is what makes the app truly multi-tenant — one strategy, N IdPs.

import passport from 'passport';
import { MultiSamlStrategy } from '@node-saml/passport-saml';
import { tenants, samlConfigs, attrMappings, users } from '../db.js';
import { mapAttributes } from './attributeMapper.js';
import { config } from '../config.js';

function spEntityId(slug) { return `${config.appBaseUrl}/sp/${slug}`; }
function acsUrl(slug) { return `${config.appBaseUrl}/callback/${slug}`; }

export function buildSamlStrategy() {
  const strategy = new MultiSamlStrategy(
    {
      passReqToCallback: true,
      getSamlOptions(req, done) {
        try {
          const slug = req.params.slug;
          const tenant = tenants.getBySlug(slug);
          if (!tenant) return done(new Error(`Unknown tenant: ${slug}`));

          const cfg = samlConfigs.getByTenantId(tenant.id);
          if (!cfg) return done(new Error(`Tenant '${slug}' has no SAML configuration yet`));

          done(null, {
            entryPoint: cfg.sso_url,
            logoutUrl: cfg.slo_url || undefined,
            issuer: spEntityId(slug),
            callbackUrl: acsUrl(slug),
            idpCert: cfg.x509_cert,
            identifierFormat: cfg.nameid_format,
            wantAssertionsSigned: true,
            wantAuthnResponseSigned: false,
            acceptedClockSkewMs: 5000,
            disableRequestedAuthnContext: true,
          });
        } catch (err) {
          done(err);
        }
      },
    },
    // verify (login response)
    (req, profile, done) => {
      try {
        const tenant = tenants.getBySlug(req.params.slug);
        const mappings = attrMappings.getByTenantId(tenant.id);
        const mapped = mapAttributes(profile, mappings);

        const user = users.upsertFromSaml({
          tenant_id: tenant.id,
          external_id: mapped.externalId,
          email: mapped.email,
          display_name: mapped.displayName,
          first_name: mapped.firstName,
          last_name: mapped.lastName,
          roles_json: JSON.stringify(mapped.roles),
          raw_attrs_json: JSON.stringify(profile),
        });
        done(null, user);
      } catch (err) {
        done(err);
      }
    },
    // verify (logout response) — local logout only in the pilot.
    (_req, _profile, done) => done(null, {}),
  );

  passport.use('saml', strategy);

  passport.serializeUser((user, done) => done(null, user.id));
  passport.deserializeUser((id, done) => {
    try {
      const u = users.findById(id);
      done(null, u || false);
    } catch (err) { done(err); }
  });

  return strategy;
}

export { spEntityId, acsUrl };
