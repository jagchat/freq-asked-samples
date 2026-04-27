// Look up a tenant by the domain part of an email address.
// Returns the tenant row or null.

import { tenants } from '../db.js';

export function resolveTenantByEmail(email) {
  if (!email || typeof email !== 'string' || !email.includes('@')) return null;
  const domain = email.split('@')[1].toLowerCase().trim();
  if (!domain) return null;
  return tenants.getByEmailDomain(domain);
}
