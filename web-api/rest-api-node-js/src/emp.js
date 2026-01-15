const express = require('express');
const router = express.Router();

module.exports = (pool) => {
  // CREATE
  router.post('/', async (req, res) => {
    const { emp_id, first_name, last_name, dept_id } = req.body;
    try {
      const result = await pool.query(
        'INSERT INTO demo_ops.emp_info (emp_id, first_name, last_name, dept_id, created_by, updated_by) VALUES ($1, $2, $3, $4, $5, $6) RETURNING *',
        [emp_id, first_name, last_name, dept_id, 'api', 'api']
      );
      res.status(201).json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // READ ALL
  router.get('/', async (req, res) => {
    try {
      const result = await pool.query('SELECT * FROM demo_ops.emp_info');
      res.json(result.rows);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // READ ONE
  router.get('/:id', async (req, res) => {
    try {
      const result = await pool.query('SELECT * FROM demo_ops.emp_info WHERE emp_id = $1', [req.params.id]);
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // UPDATE
  router.put('/:id', async (req, res) => {
    const { first_name, last_name, dept_id } = req.body;
    try {
      const result = await pool.query(
        'UPDATE demo_ops.emp_info SET first_name = $1, last_name = $2, dept_id = $3, updated_by = $4, update_date = timezone(\'utc\', now()) WHERE emp_id = $5 RETURNING *',
        [first_name, last_name, dept_id, 'api', req.params.id]
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
      const result = await pool.query('DELETE FROM demo_ops.emp_info WHERE emp_id = $1 RETURNING *', [req.params.id]);
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  return router;
};
