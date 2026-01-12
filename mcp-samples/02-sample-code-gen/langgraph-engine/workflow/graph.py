"""
LangGraph Workflow Graph

This module wires together all the workflow nodes into a complete LangGraph StateGraph.

The workflow follows this linear path:
  START → parse_intent → select_examples → plan_generation → generate_code → END

Each node:
1. Receives the current state
2. Performs its operation
3. Returns updates to add to the state
4. LangGraph automatically merges these updates into the state

Key LangGraph Concepts Used:
- **StateGraph**: A directed graph where nodes are functions and edges connect them
- **State**: A TypedDict that flows through the workflow, accumulating data
- **Nodes**: Functions that take state and return state updates
- **Edges**: Connections between nodes that determine execution order
- **Entry Point**: Where the workflow starts (parse_intent)
- **Exit Point**: Where the workflow ends (END)
- **State Merging**: LangGraph automatically merges node returns into state

"""

import logging
from langgraph.graph import StateGraph, END
from config import get_llm, get_config
from workflow.state import GeneratorState
from workflow.nodes import (
    parse_intent,
    select_examples,
    plan_generation,
    generate_code
)

logger = logging.getLogger(__name__)


# ============================================================================
# Workflow Creation
# ============================================================================

def create_workflow() -> StateGraph:
    """
    Create and configure the LangGraph workflow.

    This function:
    - Creates and compiles the complete StateGraph workflow
    - Creates a StateGraph with GeneratorState
    - Initializes the LLM client and configuration
    - Adds all workflow nodes
    - Defines linear execution path with edges
    - Connects nodes with edges (defines execution order)
    - Compiles the graph into an executable workflow

    Returns:
        Compiled LangGraph workflow ready to execute

    Workflow Structure:
        START
          ↓
        parse_intent (extracts entity name, operation type, etc.)
          ↓
        select_examples (loads relevant code examples)
          ↓
        plan_generation (decides which files to create)
          ↓
        generate_code (generates actual code)
          ↓
        END

    Example usage:
        workflow = create_workflow()
        initial_state = create_initial_state(
            user_request="Create a payments service",
            target_stack="dotnet",
            target_path="src"
        )
        result = workflow.invoke(initial_state)
        files = result["generated_files"]
    """
    logger.info("Creating LangGraph workflow...")

    # -------------------------------------------------------------------------
    # Initialize Dependencies
    # -------------------------------------------------------------------------

    # Get LLM client (used by parse_intent, plan_generation, generate_code)
    llm = get_llm()
    logger.info(f"LLM client initialized: {type(llm).__name__}")

    # Get configuration (used by select_examples)
    config = get_config()
    logger.info(f"Config loaded: examples_path={config.examples_path}")

    # -------------------------------------------------------------------------
    # Create StateGraph
    # -------------------------------------------------------------------------

    # Create the graph with our state model
    workflow = StateGraph(GeneratorState)
    logger.info("StateGraph created")

    # -------------------------------------------------------------------------
    # Add Nodes
    # -------------------------------------------------------------------------

    # Node 1: Parse Intent
    # Extracts structured information from the user's natural language request
    workflow.add_node(
        "parse_intent",
        lambda state: parse_intent(state, llm)
    )
    logger.info("Added node: parse_intent")

    # Node 2: Select Examples
    # Loads relevant code examples based on stack and parsed intent
    workflow.add_node(
        "select_examples",
        lambda state: select_examples(
            state,
            examples_path=config.examples_path,
            max_tokens=config.max_examples_tokens
        )
    )
    logger.info("Added node: select_examples")

    # Node 3: Plan Generation
    # Creates a plan for which files to generate
    workflow.add_node(
        "plan_generation",
        lambda state: plan_generation(state, llm)
    )
    logger.info("Added node: plan_generation")

    # Node 4: Generate Code
    # Generates actual code using LLM and examples
    workflow.add_node(
        "generate_code",
        lambda state: generate_code(state, llm)
    )
    logger.info("Added node: generate_code")

    # -------------------------------------------------------------------------
    # Define Workflow Path (Connect Nodes)
    # -------------------------------------------------------------------------

    # Set entry point
    workflow.set_entry_point("parse_intent")

    # Linear workflow: each node connects to the next
    workflow.add_edge("parse_intent", "select_examples")
    workflow.add_edge("select_examples", "plan_generation")
    workflow.add_edge("plan_generation", "generate_code")

    # Set exit point
    workflow.add_edge("generate_code", END)

    logger.info("Workflow edges configured")

    # -------------------------------------------------------------------------
    # Compile Workflow
    # -------------------------------------------------------------------------

    compiled_workflow = workflow.compile()
    logger.info("✓ Workflow compiled successfully")

    return compiled_workflow


# ============================================================================
# Convenience Function
# ============================================================================

def run_workflow(user_request: str, target_stack: str, target_path: str = "src") -> dict:
    """
    Convenience function to run the entire workflow in one call.

    This is a helper that:
    1. Creates the initial state
    2. Creates the workflow
    3. Runs the workflow
    4. Returns the final state (with generated files, errors, etc.)

    Args:
        user_request: User's natural language prompt
        target_stack: Technology stack (dotnet, python, javascript)
        target_path: Base path for generated files (default: "src")

    Returns:
        Final state dict with generated_files and other fields

    Example:
        result = run_workflow(
            user_request="Create a payments service with CRUD",
            target_stack="dotnet",
            target_path="src"
        )
        files = result["generated_files"]
        for file in files:
            print(f"Generated: {file['path']}")
    """
    from workflow.state import create_initial_state

    logger.info("=" * 60)
    logger.info("Running LangGraph Workflow")
    logger.info("=" * 60)
    logger.info(f"Request: {user_request}")
    logger.info(f"Stack: {target_stack}")
    logger.info(f"Path: {target_path}")

    # Create initial state
    initial_state = create_initial_state(
        user_request=user_request,
        target_stack=target_stack,
        target_path=target_path
    )

    # Create and run workflow
    workflow = create_workflow()
    final_state = workflow.invoke(initial_state)

    logger.info("=" * 60)
    logger.info("Workflow Complete")
    logger.info(f"Files Generated: {len(final_state.get('generated_files', []))}")
    logger.info(f"Errors: {len(final_state.get('errors', []))}")
    logger.info("=" * 60)

    return final_state


# ============================================================================
# Usage Notes
# ============================================================================

"""
How to use this workflow:

1. Import and create the workflow:

   from workflow.graph import create_workflow
   from workflow.state import create_initial_state

   workflow = create_workflow()

2. Create initial state:

   state = create_initial_state(
       user_request="Create a payments service with CRUD operations",
       target_stack="dotnet",
       target_path="src"
   )

3. Run the workflow:

   result = workflow.invoke(state)

4. Access results:

   files = result["generated_files"]
   for file in files:
       print(f"{file['action']}: {file['path']}")
       print(file['content'][:100])  # Preview

5. Or use the convenience function:

   from workflow.graph import run_workflow

   result = run_workflow(
       user_request="Create a payments service",
       target_stack="dotnet"
   )

Integration with FastAPI:
-------------------------

In server.py:

    from workflow.graph import run_workflow

    @app.post("/generate")
    async def generate(request: GenerateRequest):
        result = run_workflow(
            user_request=request.prompt,
            target_stack=request.stack,
            target_path=request.target_path
        )

        return {
            "success": len(result["errors"]) == 0,
            "files": result["generated_files"],
            "errors": result["errors"]
        }

This workflow is now ready to be integrated into the FastAPI server!
"""
