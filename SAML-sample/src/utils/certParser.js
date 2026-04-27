// Extracts a few useful fields from a PEM-encoded X.509 certificate.
// Uses Node's built-in crypto.X509Certificate (no extra dep).

import { X509Certificate } from 'node:crypto';

// Accepts either a full PEM string or a bare base64 block (some .cer exports).
// Returns the normalized PEM plus a few display fields.
export function parseCert(input) {
  const text = (input || '').trim();
  const pem = text.includes('BEGIN CERTIFICATE') ? text : base64ToPem(text);
  const cert = new X509Certificate(pem);
  return {
    pem,
    subject: cert.subject,
    issuer: cert.issuer,
    validFrom: cert.validFrom,
    validTo: cert.validTo,
    fingerprintSha256: cert.fingerprint256,
  };
}

// passport-saml accepts the cert either as full PEM or as the base64 block only.
// We store PEM; this strips headers if a caller needs the bare base64.
export function pemToBase64(pem) {
  return pem
    .replace(/-----BEGIN CERTIFICATE-----/g, '')
    .replace(/-----END CERTIFICATE-----/g, '')
    .replace(/\s+/g, '');
}

// Wrap a bare base64 block (as found inside SAML metadata <X509Certificate>)
// into proper PEM so the DB always stores a consistent shape.
export function base64ToPem(b64) {
  const clean = b64.replace(/\s+/g, '');
  const lines = clean.match(/.{1,64}/g)?.join('\n') ?? clean;
  return `-----BEGIN CERTIFICATE-----\n${lines}\n-----END CERTIFICATE-----`;
}
