/**
 * User controller module
 *
 * Defines Express routes and handlers for user operations
 */

const express = require('express');

/**
 * Creates an Express router with user endpoints
 * @param {UserService} userService - User service instance
 * @returns {express.Router} Configured Express router
 */
function createUserController(userService) {
  const router = express.Router();

  /**
   * GET /api/users
   * Get all users
   */
  router.get('/', async (req, res) => {
    try {
      const users = await userService.getAll();
      res.status(200).json(users);
    } catch (error) {
      console.error('Error getting all users:', error);
      res.status(500).json({
        message: 'Error retrieving users',
        error: error.message
      });
    }
  });

  /**
   * GET /api/users/:id
   * Get user by ID
   */
  router.get('/:id', async (req, res) => {
    try {
      const id = parseInt(req.params.id, 10);
      if (isNaN(id)) {
        return res.status(400).json({
          message: 'Invalid user ID'
        });
      }

      const user = await userService.getById(id);
      res.status(200).json(user);
    } catch (error) {
      console.error(`Error getting user ${req.params.id}:`, error);

      if (error.message.includes('not found')) {
        return res.status(404).json({
          message: error.message
        });
      }

      res.status(500).json({
        message: 'Error retrieving user',
        error: error.message
      });
    }
  });

  /**
   * POST /api/users
   * Create a new user
   */
  router.post('/', async (req, res) => {
    try {
      const userData = req.body;

      // Basic validation
      if (!userData || typeof userData !== 'object') {
        return res.status(400).json({
          message: 'Invalid request body'
        });
      }

      const user = await userService.create(userData);
      res.status(201).json(user);
    } catch (error) {
      console.error('Error creating user:', error);

      if (error.message.includes('Validation failed') ||
          error.message.includes('already exists')) {
        return res.status(400).json({
          message: error.message
        });
      }

      res.status(500).json({
        message: 'Error creating user',
        error: error.message
      });
    }
  });

  /**
   * PUT /api/users/:id
   * Update an existing user
   */
  router.put('/:id', async (req, res) => {
    try {
      const id = parseInt(req.params.id, 10);
      if (isNaN(id)) {
        return res.status(400).json({
          message: 'Invalid user ID'
        });
      }

      const userData = req.body;
      if (!userData || typeof userData !== 'object') {
        return res.status(400).json({
          message: 'Invalid request body'
        });
      }

      const user = await userService.update(id, userData);
      res.status(200).json(user);
    } catch (error) {
      console.error(`Error updating user ${req.params.id}:`, error);

      if (error.message.includes('not found')) {
        return res.status(404).json({
          message: error.message
        });
      }

      if (error.message.includes('Validation failed') ||
          error.message.includes('already in use')) {
        return res.status(400).json({
          message: error.message
        });
      }

      res.status(500).json({
        message: 'Error updating user',
        error: error.message
      });
    }
  });

  /**
   * DELETE /api/users/:id
   * Delete a user
   */
  router.delete('/:id', async (req, res) => {
    try {
      const id = parseInt(req.params.id, 10);
      if (isNaN(id)) {
        return res.status(400).json({
          message: 'Invalid user ID'
        });
      }

      await userService.delete(id);
      res.status(204).send();
    } catch (error) {
      console.error(`Error deleting user ${req.params.id}:`, error);

      if (error.message.includes('not found')) {
        return res.status(404).json({
          message: error.message
        });
      }

      res.status(500).json({
        message: 'Error deleting user',
        error: error.message
      });
    }
  });

  /**
   * GET /api/users/email/:email
   * Find user by email
   */
  router.get('/email/:email', async (req, res) => {
    try {
      const email = req.params.email;
      if (!email) {
        return res.status(400).json({
          message: 'Email parameter is required'
        });
      }

      const user = await userService.findByEmail(email);
      if (!user) {
        return res.status(404).json({
          message: `User with email ${email} not found`
        });
      }

      res.status(200).json(user);
    } catch (error) {
      console.error(`Error finding user by email ${req.params.email}:`, error);
      res.status(500).json({
        message: 'Error searching for user',
        error: error.message
      });
    }
  });

  return router;
}

module.exports = { createUserController };
