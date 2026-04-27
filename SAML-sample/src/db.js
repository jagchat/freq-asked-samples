// Opens the SQLite file and exposes small query helpers.
// Uses better-sqlite3 (synchronous — simpler than async drivers for a pilot).

import path from 'node:path';
import { fileURLToPath } from 'node:url';
import Database from 'better-sqlite3';
import { config } from './config.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '..');
const dbPath = path.resolve(projectRoot, config.dbFile);

export const db = new Database(dbPath);
db.pragma('journal_mode = WAL');
db.pragma('foreign_keys = ON');

export const tenants = {
  list: () => db.prepare(`
    SELECT t.*, CASE WHEN s.tenant_id IS NULL THEN 0 ELSE 1 END AS saml_configured
    FROM tenants t LEFT JOIN saml_configs s ON s.tenant_id = t.id
    ORDER BY t.created_at DESC
  `).all(),

  getBySlug: (slug) => db.prepare('SELECT * FROM tenants WHERE slug = ?').get(slug),

  getById: (id) => db.prepare('SELECT * FROM tenants WHERE id = ?').get(id),

  getByEmailDomain: (domain) => db.prepare('SELECT * FROM tenants WHERE email_domain = ?').get(domain),

  create: ({ slug, name, email_domain }) => db.prepare(
    'INSERT INTO tenants (slug, name, email_domain) VALUES (?, ?, ?)'
  ).run(slug, name, email_domain || null),

  deleteBySlug: (slug) => db.prepare('DELETE FROM tenants WHERE slug = ?').run(slug),
};

export const samlConfigs = {
  getByTenantId: (tenantId) => db.prepare('SELECT * FROM saml_configs WHERE tenant_id = ?').get(tenantId),

  upsert: (cfg) => db.prepare(`
    INSERT INTO saml_configs (tenant_id, idp_entity_id, sso_url, slo_url, x509_cert, nameid_format, updated_at)
    VALUES (@tenant_id, @idp_entity_id, @sso_url, @slo_url, @x509_cert, @nameid_format, datetime('now'))
    ON CONFLICT(tenant_id) DO UPDATE SET
      idp_entity_id=excluded.idp_entity_id,
      sso_url=excluded.sso_url,
      slo_url=excluded.slo_url,
      x509_cert=excluded.x509_cert,
      nameid_format=excluded.nameid_format,
      updated_at=datetime('now')
  `).run(cfg),
};

export const attrMappings = {
  getByTenantId: (tenantId) => db.prepare(
    'SELECT canonical_field, source_claim FROM tenant_attribute_mappings WHERE tenant_id = ?'
  ).all(tenantId),

  replaceAll: db.transaction((tenantId, mappings) => {
    db.prepare('DELETE FROM tenant_attribute_mappings WHERE tenant_id = ?').run(tenantId);
    const ins = db.prepare(
      'INSERT INTO tenant_attribute_mappings (tenant_id, canonical_field, source_claim) VALUES (?, ?, ?)'
    );
    for (const m of mappings) {
      if (m.canonical_field && m.source_claim) {
        ins.run(tenantId, m.canonical_field, m.source_claim);
      }
    }
  }),
};

export const users = {
  findById: (id) => db.prepare('SELECT * FROM users WHERE id = ?').get(id),

  upsertFromSaml: ({ tenant_id, external_id, email, display_name, first_name, last_name, roles_json, raw_attrs_json }) => {
    const existing = db.prepare(
      'SELECT id FROM users WHERE tenant_id = ? AND external_id = ?'
    ).get(tenant_id, external_id);

    if (existing) {
      db.prepare(`
        UPDATE users SET email=?, display_name=?, first_name=?, last_name=?, roles_json=?, raw_attrs_json=?, last_login=datetime('now')
        WHERE id=?
      `).run(email, display_name, first_name, last_name, roles_json, raw_attrs_json, existing.id);
      return db.prepare('SELECT * FROM users WHERE id = ?').get(existing.id);
    }

    const result = db.prepare(`
      INSERT INTO users (tenant_id, external_id, email, display_name, first_name, last_name, roles_json, raw_attrs_json, last_login)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, datetime('now'))
    `).run(tenant_id, external_id, email, display_name, first_name, last_name, roles_json, raw_attrs_json);
    return db.prepare('SELECT * FROM users WHERE id = ?').get(result.lastInsertRowid);
  },
};
