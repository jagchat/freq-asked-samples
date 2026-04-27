// Per-IdP attribute-name presets for the admin UI's "Load preset" dropdown.
// Azure AD / Okta / Auth0 defaults are stable. FusionAuth / Keycloak / authentik
// use configurable attribute mappers, so these are common defaults — operators
// should verify against their actual IdP config.

export const PRESETS = {
  generic: {
    label: 'Generic',
    map: {
      email: 'email',
      displayName: 'displayName',
      firstName: 'firstName',
      lastName: 'lastName',
      roles: 'roles',
    },
  },
  azure: {
    label: 'Azure AD',
    map: {
      email: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
      displayName: 'http://schemas.microsoft.com/identity/claims/displayname',
      firstName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname',
      lastName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname',
      roles: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    },
  },
  okta: {
    label: 'Okta',
    map: {
      email: 'email',
      displayName: 'displayName',
      firstName: 'firstName',
      lastName: 'lastName',
      roles: 'groups',
    },
  },
  auth0: {
    label: 'Auth0',
    map: {
      email: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
      displayName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
      firstName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname',
      lastName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname',
      roles: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role',
    },
  },
  fusionauth: {
    label: 'FusionAuth',
    map: {
      email: 'email',
      displayName: 'name',
      firstName: 'first_name',
      lastName: 'last_name',
      roles: 'roles',
    },
  },
  keycloak: {
    label: 'Keycloak',
    map: {
      email: 'email',
      displayName: 'name',
      firstName: 'firstName',
      lastName: 'lastName',
      roles: 'Role',
    },
  },
  authentik: {
    label: 'authentik',
    map: {
      email: 'email',
      displayName: 'displayName',
      firstName: 'givenName',
      lastName: 'surName',
      roles: 'groups',
    },
  },
};

export const CANONICAL_FIELDS = ['email', 'displayName', 'firstName', 'lastName', 'roles'];
