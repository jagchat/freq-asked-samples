# SAML-sample

A pilot multi-tenant SaaS web app where each tenant authenticates via its own SAML
Identity Provider (IdP) — Auth0, Okta, Azure AD, Keycloak, FusionAuth, authentik, etc.
One strategy, N IdPs, per-tenant config in SQLite.

- **Stack**: Node.js 20 (plain JS, ESM), Express, `@node-saml/passport-saml`, SQLite via
  `better-sqlite3` (no ORM, raw SQL), `express-session` in the same SQLite file,
  EJS views with Pico.css.
- **Auth model**: HTTP Basic Auth for `/admin` (operator); cookie session for end users
  after SAML login; same cookie authenticates `/api/me`.

---

## Prerequisites

- Node.js 20 or newer
- Windows, macOS, or Linux
- On Windows, `better-sqlite3` ships a prebuilt binary — no compiler needed

---

## First-time setup

```bash
cd SAML-sample

# 1. Review .env and set real values (especially SESSION_SECRET and ADMIN_PASSWORD).
#    The shipped .env has placeholder values that are fine for localhost testing.

# 2. Install dependencies (once)
npm install

# 3. Create the SQLite database and apply the schema (once)
npm run db:init
```

`db:init` is safe to re-run — it uses `CREATE TABLE IF NOT EXISTS`. To start from
scratch, delete `data/app.db*` and run `db:init` again.

## Running

```bash
npm run dev       # with file-watching
# or
npm start         # plain node
```

Server boots on [http://localhost:3000](http://localhost:3000).

Default routes:
- `/` — redirects to `/login` or `/app` depending on session
- `/login` — public email-entry page
- `/admin` — operator UI (HTTP Basic Auth: `ADMIN_USER` / `ADMIN_PASSWORD` from `.env`)
- `/app` — post-login dashboard (requires session)
- `/api/me` — sample protected JSON endpoint

---

## Workflows

### A. Tenant registration (operator)

You (the SaaS operator) onboard a new customer tenant.

1. **Create the tenant shell**
   - Go to `http://localhost:3000/admin`, log in with the basic-auth prompt.
   - Click **+ New tenant**, fill in:
     - **Slug** — short URL-safe identifier (e.g. `acme`). Used in all tenant URLs.
     - **Name** — human-readable (e.g. `Acme Corp`).
     - **Email domain** — optional. Enables email-based discovery at `/login`
       (typing `alice@acme.com` routes to the Acme tenant automatically).
   - On save you land on the tenant detail page.

2. **Send SP info to the customer's IdP admin**

   The tenant detail page shows three URLs the customer will need when they
   configure their IdP (Okta / Azure / Auth0 / etc.):
   - **SP Entity ID** — who your app is, to their IdP
   - **ACS URL** — where the SAML response gets POSTed
   - **SP Metadata URL** — same info as XML (some IdPs can consume this directly)

   Each has a **Copy** button.

3. **Customer configures their IdP** (happens outside this app)

   Their admin creates a SAML application in their IdP, enters your SP Entity ID
   and ACS URL, and exports their IdP metadata. They send it back to you — either
   as a metadata XML file, or as three fields (IdP Entity ID, SSO URL, signing cert).

4. **Register the IdP config**

   On the tenant detail page, click **Configure SAML**. You have two modes:
   - **Upload metadata XML** — paste the customer's `.xml`; the app extracts all
     fields for you.
   - **Enter manually** — fill in IdP Entity ID, SSO URL, NameID format, upload
     the signing certificate (`.pem` / `.cer` / `.crt`).

   Then fill in the **Attribute mapping** — use the preset dropdown for common
   IdPs (Azure AD, Okta, Auth0, FusionAuth, Keycloak, authentik, Generic), or
   type claim names manually. Click **+ Add custom attribute** to map extra
   claims beyond the five standard fields (email, displayName, firstName,
   lastName, roles).

   Save → tenant shows a green **Configured** badge.

5. **Test the round-trip**

   Click **Test login** on the tenant detail page (opens `/login/:slug` in a
   new tab). Authenticate at the IdP; you should land on `/app` with a session.

### B. User login (end user)

1. User visits `http://localhost:3000/login`.
2. Types their work email (e.g. `alice@acme.com`) and clicks **Continue**.
3. The app looks up `acme.com` → finds tenant `acme` → redirects to `/login/acme`.
4. `/login/acme` initiates SAML: the app builds a signed AuthnRequest, redirects
   the browser to the tenant's IdP SSO URL.
5. User authenticates at the IdP (may be already signed in via SSO there).
6. IdP POSTs a signed SAML response to `/callback/acme`.
7. The app:
   - Looks up Acme's SAML config (cert, expected issuer).
   - Verifies the signature against that cert.
   - Maps the SAML attributes to canonical fields using Acme's attribute map.
   - Upserts the `users` row and sets a cookie session.
8. Redirects to `/app`, which shows the dashboard with profile + raw attributes.
9. Any subsequent request to `/app` or `/api/me` carries the cookie → session →
   user is recognized without re-authenticating.

If the email domain isn't recognized, the login page shows a dev-mode
**Sign in by tenant** dropdown so you can pick one directly.

Logout: **Log out** button in the nav → `POST /logout` → session destroyed →
back to `/login`. (This is local logout only. True Single Logout that kills
the IdP session too is a future enhancement.)

---

## File layout

```
SAML-sample/
├─ .env                          # runtime config (session secret, admin creds, DB path)
├─ README.md                     # this file
├─ CLAUDE.md                     # project notes for Claude Code sessions
├─ package.json
├─ data/                         # (gitignored) SQLite database lives here
├─ scripts/
│  └─ db-init.js                 # creates data/app.db and applies src/schema.sql
└─ src/
   ├─ server.js                  # wires everything together, starts HTTP listener
   ├─ config.js                  # reads .env into a typed-ish config object
   ├─ db.js                      # opens SQLite, exposes tenants/samlConfigs/users/attrMappings helpers
   ├─ schema.sql                 # single-file DB schema (CREATE IF NOT EXISTS)
   ├─ auth/
   │  ├─ samlStrategyResolver.js # MultiSamlStrategy + per-request tenant lookup — the core of multi-tenancy
   │  ├─ attributeMapper.js      # raw SAML profile + map → canonical user object
   │  └─ tenantResolver.js       # email address → tenant row (for discovery)
   ├─ middleware/
   │  ├─ requireAuth.js          # guards /app and /api/* — redirects to /login if no session
   │  └─ requireAdmin.js         # HTTP Basic Auth for /admin/*
   ├─ routes/
   │  ├─ auth.js                 # /login, /login/:slug, /callback/:slug, /metadata/:slug, /logout
   │  ├─ admin.js                # /admin, tenant CRUD, SAML config (incl. file uploads)
   │  └─ app.js                  # /app dashboard + /api/me
   ├─ utils/
   │  ├─ presets.js              # IdP attribute-name presets for the admin UI
   │  ├─ certParser.js           # node:crypto.X509Certificate helpers + PEM normalization
   │  └─ metadataParser.js       # extracts fields from IdP metadata XML via fast-xml-parser
   └─ views/                     # EJS templates
      ├─ login.ejs
      ├─ dashboard.ejs
      ├─ partials/
      │  ├─ header.ejs
      │  └─ footer.ejs
      └─ admin/
         ├─ index.ejs            # tenants list
         ├─ tenant-new.ejs       # create tenant form
         ├─ tenant-detail.ejs    # tenant page with SP URLs + SAML status
         └─ saml-config.ejs      # three-section form: IdP conn / cert / attribute map
```

## Data model (5 tables)

- `tenants(id, slug, name, email_domain, created_at)`
- `saml_configs(tenant_id, idp_entity_id, sso_url, slo_url, x509_cert, nameid_format, updated_at)` — 1:1 with tenant
- `tenant_attribute_mappings(tenant_id, canonical_field, source_claim)` — N rows per tenant
- `users(id, tenant_id, external_id, email, display_name, first_name, last_name, roles_json, raw_attrs_json, created_at, last_login)`
- `sessions` — auto-created and managed by `better-sqlite3-session-store`

---

## Registering an Auth0 app for testing

Auth0 can emulate any SAML IdP. To exercise the pilot end-to-end:

1. **Create an Auth0 account** (free tier is fine) at auth0.com.
2. **Create a Regular Web Application** (Applications → Create Application → Regular Web Applications).
3. **Enable the SAML2 Web App addon** (Applications → your app → Addons tab → SAML2 Web App).
4. In the addon's **Settings** tab:
   - **Application Callback URL**: `http://localhost:3000/callback/<your-tenant-slug>`
     (e.g. `http://localhost:3000/callback/acme`)
   - **Settings (JSON)**: leave default, or set `"audience": "http://localhost:3000/sp/acme"`
     to match the SP Entity ID the app advertises.
5. Click **Enable**.
6. **Usage** tab of the addon shows:
   - **Identity Provider Login URL** → that's your `ssoUrl`
   - **Identity Provider Issuer** → that's your `idpEntityId`
   - **Identity Provider Certificate** → download it (PEM). This is your signing cert.
   - Or download the **Identity Provider Metadata** XML to use Upload mode.
7. In the SAML-sample admin UI:
   - Create tenant `acme`
   - Go to **Configure SAML** → upload the metadata XML, OR enter the three fields
     manually and upload the cert → pick the **Auth0** preset → Save.
8. Create a test user in Auth0 (User Management → Users → Create User).
9. Click **Test login** on the tenant detail page → sign in with the Auth0 user →
   land on `/app`.

### Simulating multiple IdPs with Auth0

Create a second Auth0 Application with SAML2 addon pointing at a different tenant
slug (e.g. `globex` → `/callback/globex`). Register it as a second tenant in the
admin UI. You now have two tenants with genuinely different certs and issuers,
which exercises the per-tenant isolation code path even though the real IdP is
just Auth0 twice.

---

## Troubleshooting

| Symptom | Likely cause |
|---|---|
| `Error: SQLITE_CANTOPEN` on start | `data/` missing — run `npm run db:init` |
| `/login/:slug` says "no SAML configuration" | Tenant exists but `saml_configs` row is empty — go to admin and Configure SAML |
| SAML callback fails with signature error | Cert in DB doesn't match the IdP's actual signing key — re-upload cert or re-upload metadata |
| SAML callback fails with audience mismatch | Auth0 addon's `audience` doesn't match your SP Entity ID — set it in Auth0 addon Settings JSON |
| Attributes empty on dashboard | Attribute mapping rows don't match the claim names the IdP sends — view **Raw SAML attributes** on the dashboard and update the mapping |
| Admin UI prompts for creds repeatedly | Wrong `ADMIN_USER` / `ADMIN_PASSWORD` in `.env` |

---

## Scope and non-goals of this pilot

- Intentionally **no migrations framework** — one-file schema, re-runnable init
- Intentionally **no ORM** — raw SQL, easy to read and change
- Intentionally **no TypeScript** — plain JS
- Intentionally **no JWT** — cookie session only (JWT can be added later for machine/API clients)
- Intentionally **no full Single Logout** — local logout only
- **No CSRF tokens** on admin forms — protected by Basic Auth, but not production-grade
