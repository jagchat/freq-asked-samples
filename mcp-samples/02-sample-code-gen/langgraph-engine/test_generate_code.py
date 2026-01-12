"""
Test script for generate_code node

This script tests the generate_code node independently before integrating
it into the full workflow.

NOTE: This test requires an active LLM connection and will make API calls.

Run: python test_generate_code.py
"""

import logging
from config import get_llm, print_config_summary
from workflow.nodes.generate_code import generate_code

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

logger = logging.getLogger(__name__)

def main():
    print("\n" + "=" * 60)
    print("Testing generate_code Node")
    print("=" * 60 + "\n")

    # Show configuration
    print_config_summary()
    print()

    # Create LLM client
    try:
        llm = get_llm()
        logger.info(f"✓ LLM client created: {type(llm).__name__}")
    except Exception as e:
        logger.error(f"✗ Failed to create LLM client: {str(e)}")
        return

    # Test case with mock data that simulates outputs from all previous nodes
    # This is a simplified test with just one file
    test_case = {
        "name": "Payment Service (dotnet)",
        "state": {
            "parsed_intent": {
                "entity_name": "Payment",
                "entity_name_plural": "Payments",
                "operation_type": "crud",
                "additional_context": "service with CRUD operations"
            },
            "selected_examples": {
                "dotnet/services/UserService.cs": """using System;
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
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email is required");
            }

            return await _userRepository.CreateAsync(user);
        }
    }

    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
    }
}
"""
            },
            "generation_plan": [
                {
                    "file_type": "service",
                    "output_path": "src/services/PaymentService.cs",
                    "relevant_examples": ["dotnet/services/UserService.cs"]
                }
            ]
        }
    }

    print(f"\nTest Case: {test_case['name']}")
    print(f"  Entity: {test_case['state']['parsed_intent']['entity_name']}")
    print(f"  Files to Generate: {len(test_case['state']['generation_plan'])}")
    print()

    # Call the node
    print("⚠ This will make an API call to the LLM. Please wait...")
    print()
    result = generate_code(test_case['state'], llm)

    # Display results
    if "generated_files" in result:
        files = result["generated_files"]
        print(f"  ✓ Generation successful!")
        print(f"    Files generated: {len(files)}")
        print()

        for idx, file_info in enumerate(files, 1):
            print(f"    File {idx}: {file_info['path']}")
            print(f"      Action: {file_info['action']}")
            print(f"      Size: {len(file_info['content'])} characters")
            print()
            print("      Preview (first 500 chars):")
            print("      " + "-" * 50)
            preview = file_info['content'][:500].replace('\n', '\n      ')
            print(f"      {preview}")
            if len(file_info['content']) > 500:
                print("      ... (truncated)")
            print("      " + "-" * 50)
            print()

        # Display token usage if available
        if "token_usage" in result:
            usage = result["token_usage"]
            print(f"  Token Usage:")
            print(f"    Input tokens:  {usage.get('input_tokens', 0)}")
            print(f"    Output tokens: {usage.get('output_tokens', 0)}")
            print(f"    Total tokens:  {usage.get('total_tokens', 0)}")
            print()

    elif "errors" in result:
        print("  ✗ Generation failed!")
        print(f"    Errors: {result['errors']}")
    else:
        print("  ✗ Unexpected result format!")

    print("-" * 60)
    print("\n" + "=" * 60)
    print("Test Complete!")
    print("=" * 60)

if __name__ == "__main__":
    main()
