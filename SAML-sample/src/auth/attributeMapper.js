// Given a raw SAML profile and a tenant's attribute map, produce a canonical user object.
// The profile's keys are whatever the IdP sent (e.g. 'email' or a long claim URI);
// the map translates them to our canonical field names.

export function mapAttributes(profile, mappings) {
  const byCanonical = Object.fromEntries(mappings.map((m) => [m.canonical_field, m.source_claim]));

  const pick = (field) => {
    const claim = byCanonical[field];
    if (claim && profile[claim] !== undefined) return profile[claim];
    // Fallback: try the canonical name itself (helps Okta/FusionAuth where claim names are short).
    return profile[field];
  };

  const emailValue = pick('email') || profile.nameID;
  const rolesValue = pick('roles');
  const roles = Array.isArray(rolesValue)
    ? rolesValue
    : typeof rolesValue === 'string' && rolesValue.length > 0
      ? rolesValue.split(/[,;\s]+/).filter(Boolean)
      : [];

  return {
    externalId: profile.nameID || emailValue,
    email: emailValue,
    displayName: pick('displayName') || [pick('firstName'), pick('lastName')].filter(Boolean).join(' ') || emailValue,
    firstName: pick('firstName') || null,
    lastName: pick('lastName') || null,
    roles,
  };
}
