const { fetch } = require('undici');
const { restApiBaseUrl, restApiBearerToken } = require('./config');

function resolveUrl(pathname) {
  const normalizedPath = pathname.startsWith('/') ? pathname.slice(1) : pathname;
  const base = restApiBaseUrl.endsWith('/') ? restApiBaseUrl : restApiBaseUrl + '/';
  return new URL(normalizedPath, base).toString();
}

async function request(pathname, { method = 'GET', headers = {}, body } = {}) {
  const requestHeaders = {
    Accept: 'application/json',
    ...headers,
  };

  if (restApiBearerToken) {
    requestHeaders.Authorization = 'Bearer ' + restApiBearerToken;
  }

  let payload;
  if (body !== undefined && body !== null) {
    requestHeaders['Content-Type'] = 'application/json';
    payload = JSON.stringify(body);
  }

  const response = await fetch(resolveUrl(pathname), {
    method,
    headers: requestHeaders,
    body: payload,
  });

  const text = await response.text();
  const contentType = response.headers.get('content-type') || '';
  const isJson = contentType.includes('application/json');
  const data = isJson && text ? JSON.parse(text) : text;

  if (!response.ok) {
    const error = new Error('REST API request failed with status ' + response.status);
    error.status = response.status;
    error.data = data;
    throw error;
  }

  return data;
}

async function listDepartments() {
  return request('/api/dept');
}

async function getDepartment({ dept_id }) {
  return request('/api/dept/' + dept_id);
}

async function createDepartment({ dept_id, dept_name }) {
  return request('/api/dept', {
    method: 'POST',
    body: { dept_id, dept_name },
  });
}

async function updateDepartment({ dept_id, dept_name }) {
  return request('/api/dept/' + dept_id, {
    method: 'PUT',
    body: { dept_name },
  });
}

async function deleteDepartment({ dept_id }) {
  return request('/api/dept/' + dept_id, {
    method: 'DELETE',
  });
}

async function listEmployees() {
  return request('/api/emp');
}

async function getEmployee({ emp_id }) {
  return request('/api/emp/' + emp_id);
}

async function createEmployee({ emp_id, first_name, last_name, dept_id }) {
  return request('/api/emp', {
    method: 'POST',
    body: { emp_id, first_name, last_name, dept_id },
  });
}

async function updateEmployee({ emp_id, first_name, last_name, dept_id }) {
  return request('/api/emp/' + emp_id, {
    method: 'PUT',
    body: { first_name, last_name, dept_id },
  });
}

async function deleteEmployee({ emp_id }) {
  return request('/api/emp/' + emp_id, {
    method: 'DELETE',
  });
}

async function listAddresses() {
  return request('/api/address');
}

async function getAddress({ address_id }) {
  return request('/api/address/' + address_id);
}

async function createAddress({
  address_id,
  emp_id,
  address_line1,
  address_line2,
  city,
  state,
  zip_code,
  country,
}) {
  return request('/api/address', {
    method: 'POST',
    body: {
      address_id,
      emp_id,
      address_line1,
      address_line2,
      city,
      state,
      zip_code,
      country,
    },
  });
}

async function updateAddress({
  address_id,
  address_line1,
  address_line2,
  city,
  state,
  zip_code,
  country,
}) {
  return request('/api/address/' + address_id, {
    method: 'PUT',
    body: {
      address_line1,
      address_line2,
      city,
      state,
      zip_code,
      country,
    },
  });
}

async function deleteAddress({ address_id }) {
  return request('/api/address/' + address_id, {
    method: 'DELETE',
  });
}

module.exports = {
  request,
  listDepartments,
  getDepartment,
  createDepartment,
  updateDepartment,
  deleteDepartment,
  listEmployees,
  getEmployee,
  createEmployee,
  updateEmployee,
  deleteEmployee,
  listAddresses,
  getAddress,
  createAddress,
  updateAddress,
  deleteAddress,
};
