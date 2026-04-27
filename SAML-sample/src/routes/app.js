// Post-login app surface — dashboard + a sample JSON endpoint.

import { Router } from 'express';
import { tenants } from '../db.js';
import { requireAuth } from '../middleware/requireAuth.js';

export function appRouter() {
  const r = Router();

  r.get('/app', requireAuth, (req, res) => {
    const tenant = tenants.getById(req.user.tenant_id);
    res.render('dashboard', {
      user: req.user,
      tenant,
      rawAttrs: req.user.raw_attrs_json ? JSON.parse(req.user.raw_attrs_json) : {},
      roles: req.user.roles_json ? JSON.parse(req.user.roles_json) : [],
    });
  });

  r.get('/api/me', requireAuth, (req, res) => {
    res.json({
      id: req.user.id,
      tenantId: req.user.tenant_id,
      email: req.user.email,
      displayName: req.user.display_name,
      firstName: req.user.first_name,
      lastName: req.user.last_name,
      roles: JSON.parse(req.user.roles_json || '[]'),
      lastLogin: req.user.last_login,
    });
  });

  return r;
}
