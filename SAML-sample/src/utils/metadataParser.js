// Parses SAML 2.0 IdP metadata XML and extracts the values we need.
// Returns { idpEntityId, ssoUrl, sloUrl, x509Cert, nameIdFormat }.

import { XMLParser } from 'fast-xml-parser';
import { base64ToPem } from './certParser.js';

const parser = new XMLParser({
  ignoreAttributes: false,
  attributeNamePrefix: '@_',
  removeNSPrefix: true,
});

const REDIRECT_BINDING = 'urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect';

export function parseIdpMetadata(xml) {
  const doc = parser.parse(xml);

  const ed = doc.EntityDescriptor || doc.EntitiesDescriptor?.EntityDescriptor;
  if (!ed) throw new Error('Not a SAML EntityDescriptor');

  const idp = ed.IDPSSODescriptor;
  if (!idp) throw new Error('EntityDescriptor has no IDPSSODescriptor');

  const idpEntityId = ed['@_entityID'];

  // SingleSignOnService may be a single object or an array; prefer HTTP-Redirect if present.
  const ssoList = [].concat(idp.SingleSignOnService || []);
  const sso = ssoList.find((s) => s['@_Binding'] === REDIRECT_BINDING) || ssoList[0];
  const ssoUrl = sso?.['@_Location'];

  const sloList = [].concat(idp.SingleLogoutService || []);
  const slo = sloList.find((s) => s['@_Binding'] === REDIRECT_BINDING) || sloList[0];
  const sloUrl = slo?.['@_Location'];

  // Signing cert: find KeyDescriptor with use="signing" (or the only one present).
  const keyList = [].concat(idp.KeyDescriptor || []);
  const signingKey = keyList.find((k) => k['@_use'] === 'signing' || !k['@_use']);
  const rawCert = signingKey?.KeyInfo?.X509Data?.X509Certificate;
  const certBase64 = typeof rawCert === 'string' ? rawCert : rawCert?.['#text'];
  const x509Cert = certBase64 ? base64ToPem(certBase64) : null;

  const nameIdFormats = [].concat(idp.NameIDFormat || []);
  const nameIdFormat = nameIdFormats[0] || 'urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress';

  if (!idpEntityId || !ssoUrl || !x509Cert) {
    throw new Error('Metadata missing required fields (entityID, SSO URL, or signing certificate)');
  }

  return { idpEntityId, ssoUrl, sloUrl: sloUrl || null, x509Cert, nameIdFormat };
}
