"""
Test script for plan_generation node

This script tests the plan_generation node independently before integrating
it into the full workflow.

Run: python test_plan_generation.py
"""

import logging
from config import get_llm, print_config_summary
from workflow.nodes.plan_generation import plan_generation

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

logger = logging.getLogger(__name__)

def main():
    print("\n" + "=" * 60)
    print("Testing plan_generation Node")
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

    # Test cases with mock data that simulates outputs from previous nodes
    test_cases = [
        {
            "name": "Payment CRUD Service (dotnet)",
            "state": {
                "target_stack": "dotnet",
                "target_path": "src",
                "parsed_intent": {
                    "entity_name": "Payment",
                    "entity_name_plural": "Payments",
                    "operation_type": "crud",
                    "additional_context": "service with CRUD operations"
                },
                "selected_examples": {
                    "dotnet/services/UserService.cs": "/* UserService.cs content */",
                    "dotnet/repositories/UserRepository.cs": "/* UserRepository.cs content */",
                    "dotnet/models/User.cs": "/* User.cs content */"
                }
            }
        },
        {
            "name": "Product Read-Only API (python)",
            "state": {
                "target_stack": "python",
                "target_path": "app",
                "parsed_intent": {
                    "entity_name": "Product",
                    "entity_name_plural": "Products",
                    "operation_type": "read-only",
                    "additional_context": "read-only API endpoints"
                },
                "selected_examples": {
                    "python/services/user_service.py": "# user_service.py content",
                    "python/models/user.py": "# user.py content"
                }
            }
        },
        {
            "name": "Order Custom Service (javascript)",
            "state": {
                "target_stack": "javascript",
                "target_path": "src",
                "parsed_intent": {
                    "entity_name": "Order",
                    "entity_name_plural": "Orders",
                    "operation_type": "custom",
                    "additional_context": "custom order processing endpoints"
                },
                "selected_examples": {
                    "javascript/services/userService.js": "// userService.js content",
                    "javascript/models/user.model.js": "// user.model.js content",
                    "javascript/controllers/userController.js": "// userController.js content"
                }
            }
        }
    ]

    for i, test_case in enumerate(test_cases, 1):
        print(f"\nTest Case {i}: {test_case['name']}")
        print(f"  Stack: {test_case['state']['target_stack']}")
        print(f"  Entity: {test_case['state']['parsed_intent']['entity_name']}")
        print(f"  Operation: {test_case['state']['parsed_intent']['operation_type']}")
        print(f"  Available Examples: {len(test_case['state']['selected_examples'])}")
        print()

        # Call the node
        result = plan_generation(test_case['state'], llm)

        # Display results
        if "generation_plan" in result:
            plan = result["generation_plan"]
            print(f"  ✓ Planning successful!")
            print(f"    Files to generate: {len(plan)}")
            print()
            print("    Plan Details:")
            for idx, file_plan in enumerate(plan, 1):
                print(f"      {idx}. {file_plan['output_path']}")
                print(f"         Type: {file_plan['file_type']}")
                print(f"         Examples: {', '.join(file_plan['relevant_examples'])}")

            # Display token usage if available
            if "token_usage" in result:
                usage = result["token_usage"]
                print(f"\n  Token Usage:")
                print(f"    Input tokens:  {usage.get('input_tokens', 0)}")
                print(f"    Output tokens: {usage.get('output_tokens', 0)}")
                print(f"    Total tokens:  {usage.get('total_tokens', 0)}")
        elif "errors" in result:
            print("  ✗ Planning failed!")
            print(f"    Errors: {result['errors']}")
        else:
            print("  ✗ Unexpected result format!")

        print("-" * 60)

    print("\n" + "=" * 60)
    print("Test Complete!")
    print("=" * 60)

if __name__ == "__main__":
    main()
