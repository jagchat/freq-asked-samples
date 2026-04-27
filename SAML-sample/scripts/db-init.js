// Creates the SQLite DB file and applies src/schema.sql once.
// Safe to re-run: schema uses IF NOT EXISTS.

import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import Database from 'better-sqlite3';
import 'dotenv/config';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '..');

const dbFile = process.env.DB_FILE || 'data/app.db';
const dbPath = path.resolve(projectRoot, dbFile);
const schemaPath = path.resolve(projectRoot, 'src/schema.sql');

fs.mkdirSync(path.dirname(dbPath), { recursive: true });

const schema = fs.readFileSync(schemaPath, 'utf8');
const db = new Database(dbPath);
db.pragma('journal_mode = WAL');
db.pragma('foreign_keys = ON');
db.exec(schema);
db.close();

console.log(`DB ready at ${dbPath}`);
