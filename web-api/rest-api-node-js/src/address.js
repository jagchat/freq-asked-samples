const express = require('express');
const router = express.Router();

module.exports = (pool) => {
  // CREATE
  router.post('/', async (req, res) => {
    const { address_id, emp_id, address_line1, address_line2, city, state, zip_code, country } = req.body;
    try {
      const result = await pool.query(
        'INSERT INTO demo_ops.address_info (address_id, emp_id, address_line1, address_line2, city, state, zip_code, country, created_by, updated_by) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10) RETURNING *',
        [address_id, emp_id, address_line1, address_line2, city, state, zip_code, country, 'api', 'api']
      );
      res.status(201).json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // READ ALL
  router.get('/', async (req, res) => {
    try {
      const result = await pool.query('SELECT * FROM demo_ops.address_info');
      res.json(result.rows);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // READ ONE
  router.get('/:id', async (req, res) => {
    try {
      const result = await pool.query('SELECT * FROM demo_ops.address_info WHERE address_id = $1', [req.params.id]);
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // UPDATE
  router.put('/:id', async (req, res) => {
    const { address_line1, address_line2, city, state, zip_code, country } = req.body;
    try {
      const result = await pool.query(
        'UPDATE demo_ops.address_info SET address_line1 = $1, address_line2 = $2, city = $3, state = $4, zip_code = $5, country = $6, updated_by = $7, update_date = timezone(\'utc\', now()) WHERE address_id = $8 RETURNING *',
        [address_line1, address_line2, city, state, zip_code, country, 'api', req.params.id]
      );
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // DELETE
  router.delete('/:id', async (req, res) => {
    try {
      const result = await pool.query('DELETE FROM demo_ops.address_info WHERE address_id = $1 RETURNING *', [req.params.id]);
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  return router;
};
