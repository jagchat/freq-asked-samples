import 'dotenv/config';

export const config = {
  port: parseInt(process.env.PORT || '3000', 10),
  appBaseUrl: process.env.APP_BASE_URL || 'http://localhost:3000',
  sessionSecret: process.env.SESSION_SECRET || 'dev-insecure-secret',
  adminUser: process.env.ADMIN_USER || 'admin',
  adminPassword: process.env.ADMIN_PASSWORD || 'admin',
  dbFile: process.env.DB_FILE || 'data/app.db',
  isProd: process.env.NODE_ENV === 'production',
};
