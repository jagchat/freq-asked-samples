using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YourProject.Models;
using YourProject.Repositories;

namespace YourProject.Services
{
    /// <summary>
    /// Service for managing User entities with CRUD operations
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }
            return user;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        public async Task<User> CreateAsync(User user)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email is required");
            }

            return await _userRepository.CreateAsync(user);
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        public async Task<User> UpdateAsync(int id, User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            user.Id = id;
            return await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            await _userRepository.DeleteAsync(id);
        }
    }

    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(int id, User user);
        Task DeleteAsync(int id);
    }
}
