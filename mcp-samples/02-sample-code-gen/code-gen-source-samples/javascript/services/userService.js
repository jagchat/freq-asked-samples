/**
 * User service module
 *
 * Contains business logic for user operations
 */

const User = require('../models/User');

/**
 * Service class for managing user operations
 */
class UserService {
  /**
   * Creates a new UserService instance
   * @param {Object} userRepository - Repository for data access
   */
  constructor(userRepository) {
    this.userRepository = userRepository;
  }

  /**
   * Retrieves all users
   * @returns {Promise<User[]>} Array of all users
   * @throws {Error} If retrieval fails
   */
  async getAll() {
    try {
      return await this.userRepository.findAll();
    } catch (error) {
      throw new Error(`Failed to retrieve users: ${error.message}`);
    }
  }

  /**
   * Retrieves a user by ID
   * @param {number} id - User ID
   * @returns {Promise<User>} User object
   * @throws {Error} If user not found or retrieval fails
   */
  async getById(id) {
    if (!id || typeof id !== 'number') {
      throw new Error('Valid user ID is required');
    }

    try {
      const user = await this.userRepository.findById(id);
      if (!user) {
        throw new Error(`User with ID ${id} not found`);
      }
      return user;
    } catch (error) {
      if (error.message.includes('not found')) {
        throw error;
      }
      throw new Error(`Failed to retrieve user: ${error.message}`);
    }
  }

  /**
   * Creates a new user
   * @param {Object} userData - User data
   * @param {string} userData.name - User's name
   * @param {string} userData.email - User's email
   * @returns {Promise<User>} Created user object
   * @throws {Error} If validation fails or creation fails
   */
  async create(userData) {
    // Validate input data
    const validation = User.validate(userData);
    if (!validation.isValid) {
      throw new Error(`Validation failed: ${validation.errors.join(', ')}`);
    }

    try {
      // Check if email already exists
      const existingUser = await this.userRepository.findByEmail(userData.email);
      if (existingUser) {
        throw new Error(`User with email ${userData.email} already exists`);
      }

      // Create user
      const newUser = {
        name: userData.name.trim(),
        email: userData.email.toLowerCase().trim(),
        createdAt: new Date()
      };

      return await this.userRepository.create(newUser);
    } catch (error) {
      if (error.message.includes('already exists')) {
        throw error;
      }
      throw new Error(`Failed to create user: ${error.message}`);
    }
  }

  /**
   * Updates an existing user
   * @param {number} id - User ID
   * @param {Object} userData - Updated user data
   * @param {string} [userData.name] - Updated name
   * @param {string} [userData.email] - Updated email
   * @returns {Promise<User>} Updated user object
   * @throws {Error} If user not found, validation fails, or update fails
   */
  async update(id, userData) {
    if (!id || typeof id !== 'number') {
      throw new Error('Valid user ID is required');
    }

    try {
      // Check if user exists
      const existingUser = await this.userRepository.findById(id);
      if (!existingUser) {
        throw new Error(`User with ID ${id} not found`);
      }

      // Prepare update data
      const updateData = {};
      if (userData.name !== undefined) {
        updateData.name = userData.name.trim();
      }
      if (userData.email !== undefined) {
        updateData.email = userData.email.toLowerCase().trim();

        // Check if email is already in use by another user
        const emailExists = await this.userRepository.findByEmail(updateData.email);
        if (emailExists && emailExists.id !== id) {
          throw new Error(`Email ${updateData.email} is already in use`);
        }
      }

      // Validate updated data
      const dataToValidate = { ...existingUser, ...updateData };
      const validation = User.validate(dataToValidate);
      if (!validation.isValid) {
        throw new Error(`Validation failed: ${validation.errors.join(', ')}`);
      }

      updateData.updatedAt = new Date();

      return await this.userRepository.update(id, updateData);
    } catch (error) {
      if (error.message.includes('not found') || error.message.includes('already in use') || error.message.includes('Validation failed')) {
        throw error;
      }
      throw new Error(`Failed to update user: ${error.message}`);
    }
  }

  /**
   * Deletes a user
   * @param {number} id - User ID
   * @returns {Promise<void>}
   * @throws {Error} If user not found or deletion fails
   */
  async delete(id) {
    if (!id || typeof id !== 'number') {
      throw new Error('Valid user ID is required');
    }

    try {
      const user = await this.userRepository.findById(id);
      if (!user) {
        throw new Error(`User with ID ${id} not found`);
      }

      await this.userRepository.delete(id);
    } catch (error) {
      if (error.message.includes('not found')) {
        throw error;
      }
      throw new Error(`Failed to delete user: ${error.message}`);
    }
  }

  /**
   * Finds a user by email address
   * @param {string} email - Email address to search for
   * @returns {Promise<User|null>} User object if found, null otherwise
   * @throws {Error} If search fails
   */
  async findByEmail(email) {
    if (!email || typeof email !== 'string') {
      throw new Error('Valid email is required');
    }

    try {
      return await this.userRepository.findByEmail(email.toLowerCase().trim());
    } catch (error) {
      throw new Error(`Failed to search for user: ${error.message}`);
    }
  }
}

module.exports = UserService;
