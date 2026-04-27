// Wires everything together and starts the HTTP server.

import path from 'node:path';
import { fileURLToPath } from 'node:url';
import express from 'express';
import session from 'express-session';
import BetterSqlite3SessionStoreFactory from 'better-sqlite3-session-store';
import passport from 'passport';

import { config } from './config.js';
import { db } from './db.js';
import { buildSamlStrategy } from './auth/samlStrategyResolver.js';
import { requireAdmin } from './middleware/requireAdmin.js';
import { authRouter } from './routes/auth.js';
import { adminRouter } from './routes/admin.js';
import { appRouter } from './routes/app.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const app = express();

app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));
app.use(express.urlencoded({ extended: true }));

const SqliteStore = BetterSqlite3SessionStoreFactory(session);
app.use(session({
  store: new SqliteStore({ client: db, expired: { clear: true, intervalMs: 15 * 60 * 1000 } }),
  secret: config.sessionSecret,
  resave: false,
  saveUninitialized: false,
  cookie: {
    httpOnly: true,
    sameSite: 'lax',
    secure: config.isProd,
    maxAge: 12 * 60 * 60 * 1000,
  },
}));

buildSamlStrategy();
app.use(passport.initialize());
app.use(passport.session());

app.get('/', (req, res) => {
  if (req.isAuthenticated && req.isAuthenticated()) return res.redirect('/app');
  res.redirect('/login');
});

app.use('/', authRouter());
app.use('/', appRouter());
app.use('/admin', requireAdmin, adminRouter());

app.use((err, req, res, _next) => {
  console.error('[error]', err);
  res.status(500).send(`Error: ${err.message}`);
});

app.listen(config.port, () => {
  console.log(`SAML-sample listening on ${config.appBaseUrl} (port ${config.port})`);
});
