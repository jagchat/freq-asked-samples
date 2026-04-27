-- SAML-sample: single-file schema. Run once by scripts/db-init.js.

CREATE TABLE IF NOT EXISTS tenants (
  id           INTEGER PRIMARY KEY AUTOINCREMENT,
  slug         TEXT NOT NULL UNIQUE,
  name         TEXT NOT NULL,
  email_domain TEXT UNIQUE,
  created_at   TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS saml_configs (
  tenant_id       INTEGER PRIMARY KEY REFERENCES tenants(id) ON DELETE CASCADE,
  idp_entity_id   TEXT NOT NULL,
  sso_url         TEXT NOT NULL,
  slo_url         TEXT,
  x509_cert       TEXT NOT NULL,
  nameid_format   TEXT NOT NULL DEFAULT 'urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress',
  updated_at      TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS tenant_attribute_mappings (
  tenant_id       INTEGER NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  canonical_field TEXT NOT NULL,
  source_claim    TEXT NOT NULL,
  PRIMARY KEY (tenant_id, canonical_field)
);

CREATE TABLE IF NOT EXISTS users (
  id           INTEGER PRIMARY KEY AUTOINCREMENT,
  tenant_id    INTEGER NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  external_id  TEXT NOT NULL,
  email        TEXT NOT NULL,
  display_name TEXT,
  first_name   TEXT,
  last_name    TEXT,
  roles_json   TEXT NOT NULL DEFAULT '[]',
  raw_attrs_json TEXT,
  created_at   TEXT NOT NULL DEFAULT (datetime('now')),
  last_login   TEXT,
  UNIQUE (tenant_id, external_id)
);

CREATE INDEX IF NOT EXISTS idx_users_tenant ON users(tenant_id);
