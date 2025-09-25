const path = require('path');
const dotenv = require('dotenv');

dotenv.config({ path: path.resolve(__dirname, '..', '.env') });

function requiredEnv(key) {
  const value = process.env[key];
  if (!value) {
    throw new Error('Missing required environment variable: ' + key);
  }
  return value;
}

module.exports = {
  restApiBaseUrl: requiredEnv('REST_API_BASE_URL'),
  restApiBearerToken: process.env.REST_API_BEARER_TOKEN || null,
  serverName: process.env.MCP_SERVER_NAME || 'mcp-rest-proxy',
  serverVersion: process.env.MCP_SERVER_VERSION || '0.1.0'
};