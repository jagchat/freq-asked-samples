# CLAUDE.md — SAML-sample

A pilot multi-tenant SaaS Node.js app with per-tenant SAML authentication.

See **README.md** for setup, workflows, and Auth0 testing instructions.

## Deliberate tech choices — do not change without discussion

- **Plain JavaScript (ESM)** — not TypeScript. Do not introduce `.ts`, `tsconfig.json`,
  or TS build tooling.
- **SQLite via `better-sqlite3`** — synchronous driver, no async overhead.
- **Raw SQL, no ORM** — see `src/schema.sql` and query helpers in `src/db.js`. Do not
  add Prisma/Drizzle/Sequelize/Knex.
- **No migrations framework** — single `schema.sql`, re-runnable `db:init` script.
- **Cookie session only** — no JWT yet. Sessions live in the same SQLite file via
  `better-sqlite3-session-store`.
- **No linter/formatter** configured — keep it that way unless the user asks.

## Layout cheat sheet

- `src/server.js` — Express wiring; start here when tracing a request.
- `src/auth/samlStrategyResolver.js` — the core of multi-tenancy. `MultiSamlStrategy`
  calls `getSamlOptions(req)` on every auth request and looks up that tenant's IdP
  config from SQLite.
- `src/routes/` — three routers: `auth.js` (login/SAML/logout), `admin.js` (tenant
  CRUD + SAML config with file uploads), `app.js` (post-login dashboard).
- `src/utils/presets.js` — IdP attribute-name defaults for Azure, Okta, Auth0,
  FusionAuth, Keycloak, authentik, Generic. FusionAuth/Keycloak/authentik
  defaults are best-guesses — those IdPs use configurable mappers.
- `src/views/admin/saml-config.ejs` — the big SAML config form (three sections).

## Common tasks

```bash
npm install         # once, after clone
npm run db:init     # creates data/app.db + applies schema (safe to re-run)
npm run dev         # node --watch src/server.js
npm start           # plain node src/server.js
```

To reset the database: delete `data/` and re-run `db:init`.

## Admin auth

Admin UI at `/admin` is protected by HTTP Basic Auth. Credentials come from
`ADMIN_USER` and `ADMIN_PASSWORD` in `.env`.

## Things out of scope for this pilot

- Full SAML Single Logout (SLO) — local logout only.
- CSRF tokens on admin forms — Basic Auth is the protection.
- JWT for API clients — cookie session is the only auth mechanism.
- SP signing keypair — we don't sign outgoing requests (only verify incoming responses).
- Multi-environment config — one `.env`, one DB file.
