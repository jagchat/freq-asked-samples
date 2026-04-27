// HTTP Basic Auth for /admin. Browser shows a native credential prompt.
// Uses timing-safe comparison to avoid timing-based guessing.

import crypto from 'node:crypto';
import { config } from '../config.js';

function safeEqual(a, b) {
  const aBuf = Buffer.from(a);
  const bBuf = Buffer.from(b);
  if (aBuf.length !== bBuf.length) return false;
  return crypto.timingSafeEqual(aBuf, bBuf);
}

export function requireAdmin(req, res, next) {
  const header = req.headers.authorization || '';
  const [scheme, encoded] = header.split(' ');
  if (scheme === 'Basic' && encoded) {
    const decoded = Buffer.from(encoded, 'base64').toString('utf8');
    const sep = decoded.indexOf(':');
    const user = decoded.slice(0, sep);
    const pass = decoded.slice(sep + 1);
    if (safeEqual(user, config.adminUser) && safeEqual(pass, config.adminPassword)) {
      return next();
    }
  }
  res.set('WWW-Authenticate', 'Basic realm="SAML-sample admin"');
  res.status(401).send('Authentication required');
}
