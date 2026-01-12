"""
Test script for select_examples node

This script tests the select_examples node independently before integrating
it into the full workflow.

Run: python test_select_examples.py
"""

import logging
from config import get_config
from workflow.nodes.select_examples import select_examples

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

logger = logging.getLogger(__name__)

def main():
    print("\n" + "=" * 60)
    print("Testing select_examples Node")
    print("=" * 60 + "\n")

    # Load configuration
    config = get_config()
    print(f"Examples Path: {config.examples_path}")
    print(f"Max Tokens: {config.max_examples_tokens}")
    print()

    # Test cases
    test_cases = [
        {
            "name": "CRUD Service (dotnet)",
            "state": {
                "target_stack": "dotnet",
                "parsed_intent": {
                    "entity_name": "Payment",
                    "entity_name_plural": "Payments",
                    "operation_type": "crud",
                    "additional_context": "service with CRUD operations"
                }
            }
        },
        {
            "name": "Read-Only API (python)",
            "state": {
                "target_stack": "python",
                "parsed_intent": {
                    "entity_name": "Product",
                    "entity_name_plural": "Products",
                    "operation_type": "read-only",
                    "additional_context": "read-only API"
                }
            }
        },
        {
            "name": "Custom Service (javascript)",
            "state": {
                "target_stack": "javascript",
                "parsed_intent": {
                    "entity_name": "Order",
                    "entity_name_plural": "Orders",
                    "operation_type": "custom",
                    "additional_context": "custom endpoints"
                }
            }
        }
    ]

    for i, test_case in enumerate(test_cases, 1):
        print(f"\nTest Case {i}: {test_case['name']}")
        print(f"  Stack: {test_case['state']['target_stack']}")
        print(f"  Operation: {test_case['state']['parsed_intent']['operation_type']}")
        print()

        # Call the node
        result = select_examples(
            test_case['state'],
            examples_path=config.examples_path,
            max_tokens=config.max_examples_tokens
        )

        # Display results
        if "selected_examples" in result:
            examples = result["selected_examples"]
            print(f"  ✓ Loading successful!")
            print(f"    Files loaded: {len(examples)}")

            if examples:
                print("    Example files:")
                for file_path in examples.keys():
                    print(f"      - {file_path}")
            else:
                print("    ⚠ No example files found (check if examples exist for this stack)")

        elif "errors" in result:
            print("  ✗ Loading failed!")
            print(f"    Errors: {result['errors']}")
        else:
            print("  ✗ Unexpected result format!")

        print("-" * 60)

    print("\n" + "=" * 60)
    print("Test Complete!")
    print("=" * 60)

if __name__ == "__main__":
    main()
