"""
LangGraph State Models

This module defines the state structure that flows through the LangGraph workflow.
Think of state as a "data bucket" that gets passed from node to node, with each
node reading from it and adding to it.

Workflow flow:
  Input → Parse Intent → Select Examples → Plan Generation → Generate Code → Output
          └─ Updates state ─┴─ Updates state ─┴─ Updates state ──┴─ Updates state ─┘
"""

from typing import TypedDict, Optional, List, Literal


# ============================================================================
# File Operation Model
# ============================================================================

class FileOperation(TypedDict):
    """
    Represents a single file operation (create or update).
    
    This is what gets returned as the final output.

    Example:
        {
            "action": "create",
            "path": "src/services/PaymentService.cs",
            "content": "public class PaymentService { ... }"
        }
    """
    action: Literal["create", "update"]  # Type of operation
    path: str                            # Relative file path
    content: str                         # Full file content


# ============================================================================
# Parsed Intent Model
# ============================================================================

class ParsedIntent(TypedDict):
    """
    Represents the extracted intent from the user's natural language request.
    
    What the LLM extracts from the user's request
    
    The parse_intent node extracts this information from the user's prompt.

    Example from "Create a payments service with CRUD operations":
        {
            "entity_name": "Payment",
            "entity_name_plural": "Payments",
            "operation_type": "crud",
            "additional_context": "service with CRUD operations"
        }
    """
    entity_name: str             # Singular form (e.g., "Payment")
    entity_name_plural: str      # Plural form (e.g., "Payments")
    operation_type: str          # Type of operation: "crud", "read-only", "custom"
    additional_context: str      # Any extra details from the request


# ============================================================================
# File Plan Model
# ============================================================================

class FilePlan(TypedDict):
    """
    Plan for a single file to be generated.
    
    Also, Lists which example files to use as references.

    The plan_generation node creates a list of these to describe what files
    will be created and which examples to use as references.

    Example:
        {
            "file_type": "service",
            "output_path": "src/services/PaymentService.cs",
            "relevant_examples": ["dotnet/services/UserService.cs", "dotnet/services/OrderService.cs"]
        }
    """
    file_type: str                # Type of file: "controller", "service", "repository", "dto", etc.
    output_path: str              # Where the file will be created
    relevant_examples: List[str]  # Paths to example files to reference


# ============================================================================
# Main Generator State
# ============================================================================

class GeneratorState(TypedDict):
    """
    The main state object that flows through the LangGraph workflow.

    Each node in the workflow can:
    - READ from any field
    - UPDATE/ADD to specific fields

    This is the "data bucket" that accumulates information as it moves through:
    Parse Intent → Select Examples → Plan Generation → Generate Code

    Initial state (from API request):
        {
            "user_request": "Create a payments service with CRUD",
            "target_stack": "dotnet",
            "target_path": "src",
            # Everything else is None or empty
        }

    Final state (after workflow completes):
        {
            "user_request": "Create a payments service with CRUD",
            "target_stack": "dotnet",
            "target_path": "src",
            "parsed_intent": { entity_name: "Payment", ... },
            "selected_examples": { "dotnet/services/UserService.cs": "content..." },
            "generation_plan": [ { file_type: "service", ... }, ... ],
            "generated_files": [ { action: "create", path: "...", content: "..." } ],
            "errors": []
        }
    """

    # -------------------------------------------------------------------------
    # Input Fields (set at the start)
    # -------------------------------------------------------------------------
    user_request: str            # The user's natural language prompt
    target_stack: str            # Technology stack: "dotnet", "python", "javascript"
    target_path: str             # Base path for generated files (e.g., "src")

    # -------------------------------------------------------------------------
    # Context Fields (populated by workflow nodes)
    # -------------------------------------------------------------------------
    selected_examples: Optional[dict[str, str]]  # path -> content mapping
                                                 # Example: {"dotnet/services/UserService.cs": "file content..."}

    client_conventions: Optional[dict]           # Optional project-specific overrides
                                                 # Not implemented in v1, reserved for future

    # -------------------------------------------------------------------------
    # Processing Fields (intermediate results)
    # -------------------------------------------------------------------------
    parsed_intent: Optional[ParsedIntent]        # Extracted intent from user request
    generation_plan: Optional[List[FilePlan]]    # Plan of what files to generate

    # -------------------------------------------------------------------------
    # Output Fields (final results)
    # -------------------------------------------------------------------------
    generated_files: List[FileOperation]         # The final output: list of files to create/update
    errors: List[str]                            # Any errors encountered during generation


# ============================================================================
# Helper Functions
# ============================================================================

def create_initial_state(
    user_request: str,
    target_stack: str,
    target_path: str = "src"
) -> GeneratorState:
    """
    Create the initial state object from API request parameters.

    Converts API request parameters into initial state 

    Sets input fields, leaves everything else empty

    This converts the incoming API request into the state format that
    LangGraph expects.

    Args:
        user_request: User's natural language prompt
        target_stack: Technology stack (dotnet, python, javascript)
        target_path: Base path for generated files

    Returns:
        Initial GeneratorState with input fields set, all others empty/None

    Example:
        state = create_initial_state(
            user_request="Create a payments service with CRUD",
            target_stack="dotnet",
            target_path="src"
        )
        # Returns:
        # {
        #     "user_request": "Create a payments service with CRUD",
        #     "target_stack": "dotnet",
        #     "target_path": "src",
        #     "selected_examples": None,
        #     "client_conventions": None,
        #     "parsed_intent": None,
        #     "generation_plan": None,
        #     "generated_files": [],
        #     "errors": []
        # }
    """
    return GeneratorState(
        # Input
        user_request=user_request,
        target_stack=target_stack,
        target_path=target_path,

        # Context (to be populated)
        selected_examples=None,
        client_conventions=None,

        # Processing (to be populated)
        parsed_intent=None,
        generation_plan=None,

        # Output (start empty)
        generated_files=[],
        errors=[]
    )


def state_summary(state: GeneratorState) -> str:
    """
    Create a human-readable summary of the current state.
    Useful for debugging and logging.
    Shows what's been completed at each step
    
    Args:
        state: The current generator state

    Returns:
        String summary of the state
    """
    lines = [
        "=== Generator State Summary ===",
        f"Request: {state['user_request']}",
        f"Stack: {state['target_stack']}",
        f"Target Path: {state['target_path']}",
        "",
        f"Intent Parsed: {'✓' if state.get('parsed_intent') else '✗'}",
        f"Examples Selected: {'✓' if state.get('selected_examples') else '✗'}",
        f"Plan Created: {'✓' if state.get('generation_plan') else '✗'}",
        f"Files Generated: {len(state.get('generated_files', []))}",
        f"Errors: {len(state.get('errors', []))}",
    ]

    if state.get('parsed_intent'):
        intent = state['parsed_intent']
        lines.extend([
            "",
            "Parsed Intent:",
            f"  Entity: {intent.get('entity_name', 'N/A')}",
            f"  Operation: {intent.get('operation_type', 'N/A')}",
        ])

    if state.get('errors'):
        lines.extend([
            "",
            "Errors:",
            *[f"  - {error}" for error in state['errors']]
        ])

    return "\n".join(lines)


# ============================================================================
# Module Test
# ============================================================================

if __name__ == "__main__":
    """
    Test the state models.
    Run with: python src/state.py
    """
    print("\n" + "=" * 60)
    print("Testing State Models")
    print("=" * 60 + "\n")

    # Create initial state
    state = create_initial_state(
        user_request="Create a payments service with CRUD operations",
        target_stack="dotnet",
        target_path="src"
    )

    print("Initial State:")
    print(state_summary(state))

    # Simulate workflow updating state
    print("\n" + "-" * 60)
    print("Simulating workflow updates...")
    print("-" * 60 + "\n")

    # Simulate parse_intent node
    state['parsed_intent'] = ParsedIntent(
        entity_name="Payment",
        entity_name_plural="Payments",
        operation_type="crud",
        additional_context="service with CRUD operations"
    )

    # Simulate select_examples node
    state['selected_examples'] = {
        "dotnet/services/UserService.cs": "// Example user service code...",
        "dotnet/services/OrderService.cs": "// Example order service code..."
    }

    # Simulate plan_generation node
    state['generation_plan'] = [
        FilePlan(
            file_type="service",
            output_path="src/services/PaymentService.cs",
            relevant_examples=["dotnet/services/UserService.cs"]
        ),
        FilePlan(
            file_type="repository",
            output_path="src/repositories/PaymentRepository.cs",
            relevant_examples=["dotnet/repositories/UserRepository.cs"]
        )
    ]

    # Simulate generate_code node
    state['generated_files'] = [
        FileOperation(
            action="create",
            path="src/services/PaymentService.cs",
            content="public class PaymentService { /* generated code */ }"
        ),
        FileOperation(
            action="create",
            path="src/repositories/PaymentRepository.cs",
            content="public class PaymentRepository { /* generated code */ }"
        )
    ]

    print("Final State After Workflow:")
    print(state_summary(state))

    print("\n" + "=" * 60)
    print("✓ State models working correctly!")
    print("=" * 60 + "\n")


"""
How State Flows Through LangGraph Workflow:
--------------------------------------------

1. API Request arrives:
   { prompt: "Create payments service", stack: "dotnet", target_path: "src" }

2. Create Initial State:
   state = create_initial_state(...)

3. Node 1: parse_intent
   - Reads: state['user_request']
   - Updates: state['parsed_intent'] = { entity_name: "Payment", ... }

4. Node 2: select_examples
   - Reads: state['target_stack'], state['parsed_intent']
   - Updates: state['selected_examples'] = { "path/to/example.cs": "content..." }

5. Node 3: plan_generation
   - Reads: state['parsed_intent'], state['selected_examples']
   - Updates: state['generation_plan'] = [ { file_type: "service", ... }, ... ]

6. Node 4: generate_code
   - Reads: state['generation_plan'], state['selected_examples']
   - Updates: state['generated_files'] = [ { action: "create", path: "...", content: "..." } ]

7. Return state['generated_files'] as API response

Each node adds information, building up to the final result!
"""
