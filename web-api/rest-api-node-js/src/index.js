
require('dotenv').config();
const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const deptRoutes = require('./dept');
const empRoutes = require('./emp');
const addressRoutes = require('./address');

const app = express();
app.use(cors());
app.use(express.json());

// NOTE:
// env variable could be part of .env file or system environment
// ex: DATABASE_URL=postgres://username:password@localhost:5432/dbname
const pool = new Pool({
  connectionString: process.env.DATABASE_URL
});

app.use('/api/dept', deptRoutes(pool));
app.use('/api/emp', empRoutes(pool));
app.use('/api/address', addressRoutes(pool));

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
