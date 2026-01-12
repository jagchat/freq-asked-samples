"""
Test script for the complete LangGraph workflow

This script tests the entire workflow end-to-end, from user request
to generated code files.

NOTE: This test requires an active LLM connection and will make multiple API calls.

Run: python test_workflow.py
"""

import logging
from workflow.graph import run_workflow
from workflow.state import state_summary

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

logger = logging.getLogger(__name__)

def main():
    print("\n" + "=" * 60)
    print("Testing Complete LangGraph Workflow")
    print("=" * 60 + "\n")

    print("⚠ This test will make multiple LLM API calls.")
    print("Ensure your .env file is configured with API keys.\n")

    # Test case
    test_request = {
        "user_request": "Create a payments service with CRUD operations",
        "target_stack": "dotnet",
        "target_path": "src"
    }

    print("Test Configuration:")
    print(f"  Request: {test_request['user_request']}")
    print(f"  Stack: {test_request['target_stack']}")
    print(f"  Target Path: {test_request['target_path']}")
    print()

    try:
        print("Starting workflow...\n")

        # Run the complete workflow
        result = run_workflow(
            user_request=test_request["user_request"],
            target_stack=test_request["target_stack"],
            target_path=test_request["target_path"]
        )

        print("\n" + "=" * 60)
        print("Workflow Results")
        print("=" * 60 + "\n")

        # Display state summary
        print(state_summary(result))
        print()

        # Check for errors
        if result.get("errors"):
            print("✗ Errors occurred:")
            for error in result["errors"]:
                print(f"  - {error}")
            print()

        # Display generated files
        generated_files = result.get("generated_files", [])
        if generated_files:
            print(f"✓ Successfully generated {len(generated_files)} file(s):\n")

            for idx, file_info in enumerate(generated_files, 1):
                print(f"{idx}. {file_info['path']}")
                print(f"   Action: {file_info['action']}")
                print(f"   Size: {len(file_info['content'])} characters")
                print()

                # Show preview of each file
                print("   Preview (first 300 chars):")
                print("   " + "-" * 50)
                preview = file_info['content'][:300].replace('\n', '\n   ')
                print(f"   {preview}")
                if len(file_info['content']) > 300:
                    print("   ... (truncated)")
                print("   " + "-" * 50)
                print()

        else:
            print("✗ No files were generated!")
            print()

        # Display intermediate steps
        print("\n" + "=" * 60)
        print("Workflow Steps Verification")
        print("=" * 60 + "\n")

        # Check parsed intent
        if result.get("parsed_intent"):
            intent = result["parsed_intent"]
            print("✓ Step 1: Parse Intent")
            print(f"  Entity: {intent['entity_name']} (plural: {intent['entity_name_plural']})")
            print(f"  Operation: {intent['operation_type']}")
            print(f"  Context: {intent['additional_context']}")
            print()
        else:
            print("✗ Step 1: Parse Intent - FAILED")
            print()

        # Check selected examples
        if result.get("selected_examples"):
            examples = result["selected_examples"]
            print("✓ Step 2: Select Examples")
            print(f"  Loaded {len(examples)} example file(s):")
            for example_path in list(examples.keys())[:5]:  # Show first 5
                print(f"    - {example_path}")
            if len(examples) > 5:
                print(f"    ... and {len(examples) - 5} more")
            print()
        else:
            print("✗ Step 2: Select Examples - FAILED")
            print()

        # Check generation plan
        if result.get("generation_plan"):
            plan = result["generation_plan"]
            print("✓ Step 3: Plan Generation")
            print(f"  Planned {len(plan)} file(s):")
            for file_plan in plan:
                print(f"    - {file_plan['output_path']} ({file_plan['file_type']})")
            print()
        else:
            print("✗ Step 3: Plan Generation - FAILED")
            print()

        # Check generated files
        if generated_files:
            print("✓ Step 4: Generate Code")
            print(f"  Generated {len(generated_files)} file(s)")
            print()
        else:
            print("✗ Step 4: Generate Code - FAILED")
            print()

        print("=" * 60)
        if generated_files and not result.get("errors"):
            print("✓ Workflow Test PASSED!")
        else:
            print("✗ Workflow Test FAILED!")
        print("=" * 60)

    except Exception as e:
        print("\n" + "=" * 60)
        print("✗ Test Failed with Exception")
        print("=" * 60)
        print(f"\nError: {str(e)}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    main()
