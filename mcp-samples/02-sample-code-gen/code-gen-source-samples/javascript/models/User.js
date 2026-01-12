/**
 * User model definition
 *
 * Represents a User entity with validation
 */

/**
 * User class representing a user entity
 */
class User {
  /**
   * Creates a new User instance
   * @param {Object} data - User data
   * @param {number} data.id - Unique user identifier
   * @param {string} data.name - User's full name
   * @param {string} data.email - User's email address
   * @param {Date} data.createdAt - Creation timestamp
   * @param {Date} [data.updatedAt] - Last update timestamp
   */
  constructor({ id, name, email, createdAt, updatedAt = null }) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.createdAt = createdAt;
    this.updatedAt = updatedAt;
  }

  /**
   * Validates user data
   * @param {Object} data - Data to validate
   * @returns {Object} Validation result with isValid flag and errors array
   */
  static validate(data) {
    const errors = [];

    // Validate name
    if (!data.name || typeof data.name !== 'string') {
      errors.push('Name is required and must be a string');
    } else if (data.name.trim().length === 0) {
      errors.push('Name cannot be empty');
    } else if (data.name.length > 100) {
      errors.push('Name must not exceed 100 characters');
    }

    // Validate email
    if (!data.email || typeof data.email !== 'string') {
      errors.push('Email is required and must be a string');
    } else {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(data.email)) {
        errors.push('Email must be a valid email address');
      } else if (data.email.length > 255) {
        errors.push('Email must not exceed 255 characters');
      }
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  }

  /**
   * Converts user instance to plain object
   * @returns {Object} Plain object representation
   */
  toJSON() {
    return {
      id: this.id,
      name: this.name,
      email: this.email,
      createdAt: this.createdAt,
      updatedAt: this.updatedAt
    };
  }

  /**
   * Creates a User instance from a database row
   * @param {Object} row - Database row data
   * @returns {User} User instance
   */
  static fromDatabase(row) {
    return new User({
      id: row.id,
      name: row.name,
      email: row.email,
      createdAt: new Date(row.created_at),
      updatedAt: row.updated_at ? new Date(row.updated_at) : null
    });
  }
}

module.exports = User;
