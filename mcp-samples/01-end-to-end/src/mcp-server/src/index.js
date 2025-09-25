const { McpServer } = require('@modelcontextprotocol/sdk/server/mcp.js');
const { StdioServerTransport } = require('@modelcontextprotocol/sdk/server/stdio.js');
const { z } = require('zod');
const config = require('./config');
const {
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
} = require('./restClient');

const idSchema = z.union([z.string().min(1), z.number()]);
const optionalString = z.string().optional();

function formatJson(data) {
  return JSON.stringify(data, null, 2);
}

function wrapSuccess(data) {
  return {
    content: [
      {
        type: 'text',
        text: formatJson(data),
      },
    ],
  };
}

function wrapError(error) {
  const payload = {
    message: error.message ?? 'REST request failed',
  };

  if (error.status) {
    payload.status = error.status;
  }

  if (typeof error.data !== 'undefined') {
    payload.details = error.data;
  }

  return {
    isError: true,
    content: [
      {
        type: 'text',
        text: formatJson(payload),
      },
    ],
  };
}

function normalizeId(value) {
  return typeof value === 'number' ? String(value) : value;
}

async function registerTools(server) {
  // Departments
  server.registerTool(
    'list-departments',
    {
      title: 'List Departments',
      description: 'Fetches department records from the REST API.',
      inputSchema: {},
    },
    async () => wrapSuccess(await listDepartments()),
  );

  server.registerTool(
    'get-department',
    {
      title: 'Get Department',
      description: 'Fetches a single department record by ID.',
      inputSchema: {
        dept_id: idSchema,
      },
    },
    async ({ dept_id }) => {
      try {
        return wrapSuccess(await getDepartment({ dept_id: normalizeId(dept_id) }));
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'create-department',
    {
      title: 'Create Department',
      description: 'Creates a department record in the REST API.',
      inputSchema: {
        dept_id: idSchema,
        dept_name: z.string().min(1, 'dept_name is required'),
      },
    },
    async ({ dept_id, dept_name }) => {
      try {
        return wrapSuccess(
          await createDepartment({
            dept_id: normalizeId(dept_id),
            dept_name,
          }),
        );
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'update-department',
    {
      title: 'Update Department',
      description: 'Updates an existing department record.',
      inputSchema: {
        dept_id: idSchema,
        dept_name: z.string().min(1, 'dept_name is required'),
      },
    },
    async ({ dept_id, dept_name }) => {
      try {
        return wrapSuccess(
          await updateDepartment({
            dept_id: normalizeId(dept_id),
            dept_name,
          }),
        );
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'delete-department',
    {
      title: 'Delete Department',
      description: 'Deletes a department record.',
      inputSchema: {
        dept_id: idSchema,
      },
    },
    async ({ dept_id }) => {
      try {
        return wrapSuccess(await deleteDepartment({ dept_id: normalizeId(dept_id) }));
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  // Employees
  server.registerTool(
    'list-employees',
    {
      title: 'List Employees',
      description: 'Fetches employee records from the REST API.',
      inputSchema: {},
    },
    async () => wrapSuccess(await listEmployees()),
  );

  server.registerTool(
    'get-employee',
    {
      title: 'Get Employee',
      description: 'Fetches a single employee record by ID.',
      inputSchema: {
        emp_id: idSchema,
      },
    },
    async ({ emp_id }) => {
      try {
        return wrapSuccess(await getEmployee({ emp_id: normalizeId(emp_id) }));
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'create-employee',
    {
      title: 'Create Employee',
      description: 'Creates an employee record.',
      inputSchema: {
        emp_id: idSchema,
        first_name: z.string().min(1, 'first_name is required'),
        last_name: z.string().min(1, 'last_name is required'),
        dept_id: idSchema,
      },
    },
    async ({ emp_id, first_name, last_name, dept_id }) => {
      try {
        return wrapSuccess(
          await createEmployee({
            emp_id: normalizeId(emp_id),
            first_name,
            last_name,
            dept_id: normalizeId(dept_id),
          }),
        );
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'update-employee',
    {
      title: 'Update Employee',
      description: 'Updates an existing employee record.',
      inputSchema: {
        emp_id: idSchema,
        first_name: z.string().min(1, 'first_name is required'),
        last_name: z.string().min(1, 'last_name is required'),
        dept_id: idSchema,
      },
    },
    async ({ emp_id, first_name, last_name, dept_id }) => {
      try {
        return wrapSuccess(
          await updateEmployee({
            emp_id: normalizeId(emp_id),
            first_name,
            last_name,
            dept_id: normalizeId(dept_id),
          }),
        );
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'delete-employee',
    {
      title: 'Delete Employee',
      description: 'Deletes an employee record.',
      inputSchema: {
        emp_id: idSchema,
      },
    },
    async ({ emp_id }) => {
      try {
        return wrapSuccess(await deleteEmployee({ emp_id: normalizeId(emp_id) }));
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  // Addresses
  server.registerTool(
    'list-addresses',
    {
      title: 'List Addresses',
      description: 'Fetches address records from the REST API.',
      inputSchema: {},
    },
    async () => wrapSuccess(await listAddresses()),
  );

  server.registerTool(
    'get-address',
    {
      title: 'Get Address',
      description: 'Fetches a single address record by ID.',
      inputSchema: {
        address_id: idSchema,
      },
    },
    async ({ address_id }) => {
      try {
        return wrapSuccess(await getAddress({ address_id: normalizeId(address_id) }));
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'create-address',
    {
      title: 'Create Address',
      description: 'Creates an address record.',
      inputSchema: {
        address_id: idSchema,
        emp_id: idSchema,
        address_line1: z.string().min(1, 'address_line1 is required'),
        address_line2: optionalString,
        city: z.string().min(1, 'city is required'),
        state: z.string().min(1, 'state is required'),
        zip_code: z.string().min(1, 'zip_code is required'),
        country: z.string().min(1, 'country is required'),
      },
    },
    async ({
      address_id,
      emp_id,
      address_line1,
      address_line2,
      city,
      state,
      zip_code,
      country,
    }) => {
      try {
        return wrapSuccess(
          await createAddress({
            address_id: normalizeId(address_id),
            emp_id: normalizeId(emp_id),
            address_line1,
            address_line2,
            city,
            state,
            zip_code,
            country,
          }),
        );
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'update-address',
    {
      title: 'Update Address',
      description: 'Updates an existing address record.',
      inputSchema: {
        address_id: idSchema,
        address_line1: z.string().min(1, 'address_line1 is required'),
        address_line2: optionalString,
        city: z.string().min(1, 'city is required'),
        state: z.string().min(1, 'state is required'),
        zip_code: z.string().min(1, 'zip_code is required'),
        country: z.string().min(1, 'country is required'),
      },
    },
    async ({
      address_id,
      address_line1,
      address_line2,
      city,
      state,
      zip_code,
      country,
    }) => {
      try {
        return wrapSuccess(
          await updateAddress({
            address_id: normalizeId(address_id),
            address_line1,
            address_line2,
            city,
            state,
            zip_code,
            country,
          }),
        );
      } catch (error) {
        return wrapError(error);
      }
    },
  );

  server.registerTool(
    'delete-address',
    {
      title: 'Delete Address',
      description: 'Deletes an address record.',
      inputSchema: {
        address_id: idSchema,
      },
    },
    async ({ address_id }) => {
      try {
        return wrapSuccess(await deleteAddress({ address_id: normalizeId(address_id) }));
      } catch (error) {
        return wrapError(error);
      }
    },
  );
}

async function start() {
  const server = new McpServer({
    name: config.serverName,
    version: config.serverVersion,
    description: 'MCP server that proxies requests to the REST API.',
  });

  await registerTools(server);

  const transport = new StdioServerTransport();
  await server.connect(transport);
}

start().catch((error) => {
  console.error('Failed to start MCP server:', error);
  process.exit(1);
});
