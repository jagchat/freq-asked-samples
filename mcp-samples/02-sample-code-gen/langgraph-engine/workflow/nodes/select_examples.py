"""
Select Examples Node

This node loads relevant code examples from the code-gen-source-samples directory
based on the parsed intent and target stack.

Key Features:
- Smart Subdirectory Selection: Based on operation type, it knows which folders to prioritize
- Token Limiting: Respects MAX_EXAMPLES_TOKENS config to fit in LLM context window
- Priority Loading: Loads in order: services → repositories → models → controllers
- Error Handling: Returns errors in state if directory doesn't exist or loading fails
- Flexible: Works with any stack (dotnet, python, javascript, etc.)

Input (from state):
- target_stack: "dotnet", "python", "javascript"
- parsed_intent: {
    entity_name: "Payment",
    operation_type: "crud",
    ...
  }
- config (via examples_path): Path to code-gen-source-samples directory

Output (adds to state):
- selected_examples: {
    "dotnet/services/UserService.cs": "public class UserService { ... }",
    "dotnet/models/User.cs": "public class User { ... }",
    ...
  }

Why this matters:
- Loads real code examples that the LLM will learn from
- Filters examples based on operation type (CRUD → load services + repositories)
- Respects token limits to fit in LLM context window
- Provides the "training data" for code generation
"""

from typing import Dict
import logging
import os
from pathlib import Path

logger = logging.getLogger(__name__)


# ============================================================================
# Helper Functions
# ============================================================================

def estimate_tokens(text: str) -> int:
    """
    Rough estimate of token count.
    Uses the common approximation: 1 token ≈ 4 characters.

    For more accurate counting, you could use tiktoken library,
    but this simple approximation works well enough for our purposes.
    """
    return len(text) // 4


def load_file_safely(file_path: Path) -> str:
    """
    Load a file and return its contents.
    Returns empty string if file cannot be read.
    """
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            return f.read()
    except Exception as e:
        logger.warning(f"Could not read {file_path}: {str(e)}")
        return ""


def get_relevant_subdirs(operation_type: str) -> list[str]:
    """
    Determine which subdirectories to load examples from based on operation type.

    Args:
        operation_type: "crud", "read-only", "custom", "other"

    Returns:
        List of subdirectory names to search

    Examples:
        "crud" → ["services", "repositories", "models", "controllers"]
        "read-only" → ["services", "models"]
        "other" → ["services", "models"]
    """
    if operation_type == "crud":
        # Full CRUD needs services, repositories, models, and possibly controllers
        return ["services", "repositories", "models", "controllers"]
    elif operation_type == "read-only":
        # Read-only needs services and models
        return ["services", "models"]
    elif operation_type == "custom":
        # Custom operations need services and models at minimum
        return ["services", "models", "controllers"]
    else:
        # Default: load services and models
        return ["services", "models"]


def load_examples_from_directory(
    base_path: Path,
    stack: str,
    subdirs: list[str],
    max_tokens: int
) -> Dict[str, str]:
    """
    Load example files from specified subdirectories, respecting token limit.

    Args:
        base_path: Path to code-gen-source-samples directory
        stack: Target stack (dotnet, python, javascript)
        subdirs: List of subdirectories to search
        max_tokens: Maximum total tokens to load

    Returns:
        Dictionary of {relative_file_path: file_content}

    Strategy:
        - Walk through each subdirectory
        - Load files until we hit the token limit
        - Prioritize based on subdirectory order (services first, then repos, etc.)
    """
    examples = {}
    total_tokens = 0

    stack_path = base_path / stack

    if not stack_path.exists():
        logger.warning(f"Stack directory not found: {stack_path}")
        return examples

    logger.info(f"Loading examples from: {stack_path}")
    logger.info(f"Searching subdirectories: {subdirs}")

    # Iterate through each subdirectory in priority order
    for subdir in subdirs:
        subdir_path = stack_path / subdir

        if not subdir_path.exists():
            logger.debug(f"Subdirectory not found: {subdir_path}")
            continue

        # Walk through all files in this subdirectory
        for root, dirs, files in os.walk(subdir_path):
            for file_name in files:
                # Skip non-code files
                if file_name.startswith('.') or file_name == 'README.md':
                    continue

                file_path = Path(root) / file_name

                # Load file content
                content = load_file_safely(file_path)
                if not content:
                    continue

                # Estimate tokens
                tokens = estimate_tokens(content)

                # Check if adding this file would exceed limit
                if total_tokens + tokens > max_tokens:
                    logger.info(f"Token limit reached ({total_tokens}/{max_tokens}). Stopping.")
                    return examples

                # Add to examples
                # Store relative path from stack directory for cleaner keys
                relative_path = file_path.relative_to(base_path)
                examples[str(relative_path).replace('\\', '/')] = content
                total_tokens += tokens

                logger.info(f"Loaded: {relative_path} ({tokens} tokens, total: {total_tokens})")

    logger.info(f"Loaded {len(examples)} example file(s), total tokens: {total_tokens}")
    return examples


# ============================================================================
# Node Function
# ============================================================================

def select_examples(state: Dict, examples_path: str, max_tokens: int) -> Dict:
    """
    Load relevant code examples from the examples library.

    This node reads the parsed intent and target stack, then loads
    appropriate example files from the code-gen-source-samples directory.

    Args:
        state: Current workflow state containing target_stack and parsed_intent
        examples_path: Path to code-gen-source-samples directory (from config)
        max_tokens: Maximum tokens to load (from config)

    Returns:
        Dict with selected_examples field added

    Example:
        Input state: {
            "target_stack": "dotnet",
            "parsed_intent": {
                "entity_name": "Payment",
                "operation_type": "crud"
            }
        }

        Output: {
            "selected_examples": {
                "dotnet/services/UserService.cs": "public class UserService { ... }",
                "dotnet/repositories/UserRepository.cs": "public class UserRepository { ... }",
                "dotnet/models/User.cs": "public class User { ... }"
            }
        }
    """
    logger.info("=" * 60)
    logger.info("Node: select_examples")
    logger.info(f"Stack: {state.get('target_stack')}")
    logger.info(f"Intent: {state.get('parsed_intent')}")

    try:
        # Extract required info from state
        target_stack = state.get("target_stack")
        parsed_intent = state.get("parsed_intent", {})
        operation_type = parsed_intent.get("operation_type", "other")

        if not target_stack:
            raise ValueError("target_stack is required")

        # Resolve examples path
        base_path = Path(examples_path).resolve()

        if not base_path.exists():
            raise FileNotFoundError(f"Examples directory not found: {base_path}")

        # Determine which subdirectories to search
        subdirs = get_relevant_subdirs(operation_type)
        logger.info(f"Operation type '{operation_type}' → loading from: {subdirs}")

        # Load examples
        examples = load_examples_from_directory(
            base_path=base_path,
            stack=target_stack,
            subdirs=subdirs,
            max_tokens=max_tokens
        )

        if not examples:
            logger.warning("No examples loaded! Check if example files exist.")

        logger.info("=" * 60)

        # Return the selected examples to add to state
        return {
            "selected_examples": examples
        }

    except Exception as e:
        logger.error(f"Error selecting examples: {str(e)}", exc_info=True)
        # Return error in state
        return {
            "errors": [f"Failed to select examples: {str(e)}"]
        }


# ============================================================================
# Usage Notes
# ============================================================================

"""
How to use this node:

1. In your workflow graph:

   from workflow.nodes.select_examples import select_examples
   from config import get_config

   config = get_config()

   # Add to graph
   graph.add_node(
       "select_examples",
       lambda state: select_examples(
           state,
           examples_path=config.examples_path,
           max_tokens=config.max_examples_tokens
       )
   )

2. The node reads from state:
   - target_stack (required): "dotnet", "python", "javascript"
   - parsed_intent (required): Output from parse_intent node

3. The node adds to state:
   - selected_examples: Dict[str, str] mapping file paths to content
   - OR errors: list if loading fails

4. Next nodes can use selected_examples:
   - plan_generation uses them to decide which files to create
   - generate_code uses them as reference examples for the LLM

5. Token limiting strategy:
   - Loads files in priority order (services → repositories → models → controllers)
   - Stops when max_tokens is reached
   - Uses simple character-based estimation (1 token ≈ 4 chars)
"""
