const express = require('express');
const router = express.Router();

module.exports = (pool) => {
  // CREATE
  router.post('/', async (req, res) => {
    const { dept_id, dept_name } = req.body;
    try {
      const result = await pool.query(
        'INSERT INTO demo_ops.dept_info (dept_id, dept_name, created_by, updated_by) VALUES ($1, $2, $3, $4) RETURNING *',
        [dept_id, dept_name, 'api', 'api']
      );
      res.status(201).json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // READ ALL
  router.get('/', async (req, res) => {
    try {
      const result = await pool.query('SELECT * FROM demo_ops.dept_info');
      res.json(result.rows);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // READ ONE
  router.get('/:id', async (req, res) => {
    try {
      const result = await pool.query('SELECT * FROM demo_ops.dept_info WHERE dept_id = $1', [req.params.id]);
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  // UPDATE
  router.put('/:id', async (req, res) => {
    const { dept_name } = req.body;
    try {
      const result = await pool.query(
        'UPDATE demo_ops.dept_info SET dept_name = $1, updated_by = $2 WHERE dept_id = $3 RETURNING *',
        [dept_name, 'api', req.params.id]
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
      const result = await pool.query('DELETE FROM demo_ops.dept_info WHERE dept_id = $1 RETURNING *', [req.params.id]);
      if (result.rows.length === 0) return res.status(404).json({ error: 'Not found' });
      res.json(result.rows[0]);
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  });

  return router;
};
