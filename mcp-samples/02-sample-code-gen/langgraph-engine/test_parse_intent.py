"""
Test script for parse_intent node

This script tests the parse_intent node independently before integrating
it into the full workflow.

Run: python test_parse_intent.py
"""

import logging
from config import get_llm, print_config_summary
from workflow.nodes.parse_intent import parse_intent

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

logger = logging.getLogger(__name__)

def main():
    print("\n" + "=" * 60)
    print("Testing parse_intent Node")
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

    # Test cases
    test_cases = [
        {
            "user_request": "Create a payments service with CRUD operations",
            "target_stack": "dotnet"
        },
        {
            "user_request": "Add a users API with read and create endpoints",
            "target_stack": "python"
        },
        {
            "user_request": "Build a product catalog service",
            "target_stack": "javascript"
        }
    ]

    for i, test_case in enumerate(test_cases, 1):
        print(f"\nTest Case {i}:")
        print(f"  Request: {test_case['user_request']}")
        print(f"  Stack: {test_case['target_stack']}")
        print()

        # Call the node
        result = parse_intent(test_case, llm)

        # Display results
        if "parsed_intent" in result:
            intent = result["parsed_intent"]
            print("  ✓ Parsing successful!")
            print(f"    Entity (singular): {intent['entity_name']}")
            print(f"    Entity (plural): {intent['entity_name_plural']}")
            print(f"    Operation Type: {intent['operation_type']}")
            print(f"    Context: {intent['additional_context']}")
        elif "errors" in result:
            print("  ✗ Parsing failed!")
            print(f"    Errors: {result['errors']}")
        else:
            print("  ✗ Unexpected result format!")

        print("-" * 60)

    print("\n" + "=" * 60)
    print("Test Complete!")
    print("=" * 60)

if __name__ == "__main__":
    main()
