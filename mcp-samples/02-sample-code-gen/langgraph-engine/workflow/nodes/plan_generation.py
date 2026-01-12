"""
Plan Generation Node

This node uses an LLM to analyze the parsed intent and selected examples,
then creates a structured plan for which files should be generated.

Key Features:
- LLM-Driven Planning: Uses the LLM to intelligently decide which files to generate
- Stack-Aware: Follows naming conventions and folder structures for each stack
- Operation-Type Logic: Generates appropriate files based on CRUD, read-only, custom operations
- Example Mapping: Links each output file to the most relevant example files
- Structured Output: Returns well-structured plan ready for code generation

Note: This node requires an active LLM connection since it uses the LLM to make planning decisions.

Input (from state):
- parsed_intent: {
    entity_name: "Payment",
    entity_name_plural: "Payments",
    operation_type: "crud",
    ...
  }
- selected_examples: {
    "dotnet/services/UserService.cs": "...",
    "dotnet/models/User.cs": "...",
    ...
  }
- target_stack: "dotnet"
- target_path: "src"

Output (adds to state):
- generation_plan: [
    {
        "file_type": "service",
        "output_path": "src/services/PaymentService.cs",
        "relevant_examples": ["dotnet/services/UserService.cs"]
    },
    {
        "file_type": "model",
        "output_path": "src/models/Payment.cs",
        "relevant_examples": ["dotnet/models/User.cs"]
    },
    ...
  ]

Why this matters:
- Determines the complete file structure that will be generated
- Maps which example files are relevant for each output file
- Provides a blueprint for the generate_code node
- Allows the LLM to make intelligent decisions about project structure
"""

from typing import Dict, List
import logging
from langchain_core.language_models.chat_models import BaseChatModel
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import JsonOutputParser
from pydantic import BaseModel, Field

logger = logging.getLogger(__name__)


# ============================================================================
# Pydantic Models for Structured Output
# ============================================================================

class FilePlan(BaseModel):
    """
    Represents a plan for generating a single file.
    """
    file_type: str = Field(
        description="Type of file: 'service', 'repository', 'model', 'controller', 'config', etc."
    )
    output_path: str = Field(
        description="Complete output path including filename (e.g., 'src/services/PaymentService.cs')"
    )
    relevant_examples: List[str] = Field(
        description="List of example file paths that should be used as reference for this file"
    )


class GenerationPlanOutput(BaseModel):
    """
    Complete generation plan for all files.
    """
    files: List[FilePlan] = Field(
        description="List of files to generate, each with type, path, and relevant examples"
    )


# ============================================================================
# Prompt Template
# ============================================================================

PLAN_GENERATION_PROMPT = ChatPromptTemplate.from_messages([
    ("system", """You are an expert at planning software project structures.

Your job is to analyze a code generation request and create a detailed plan for which files should be generated.

Given:
1. The parsed intent (entity name, operation type, etc.)
2. Example code files from a library
3. The target stack (dotnet, python, javascript)
4. The target output path

You must decide:
- Which files need to be generated (services, repositories, models, controllers, etc.)
- The complete output path for each file (including proper folder structure)
- Which example files are most relevant for generating each output file

Guidelines:

**File Naming Conventions:**
- .NET: PascalCase (e.g., PaymentService.cs, Payment.cs)
- Python: snake_case (e.g., payment_service.py, payment.py)
- JavaScript: camelCase or PascalCase depending on convention (e.g., PaymentService.js, payment.model.js)

**Folder Structure:**
- Follow the pattern shown in the example files
- Common patterns:
  - .NET: services/, repositories/, models/, controllers/
  - Python: services/, repositories/, models/, api/
  - JavaScript: services/, repositories/, models/, controllers/

**Operation Type Patterns:**
- "crud": Generate service, repository, model, and optionally controller
- "read-only": Generate service and model (no repository needed)
- "custom": Generate service, model, and optionally controller
- "other": At minimum, generate service and model

**Mapping Examples:**
- Match file types carefully (service example → service output, model example → model output)
- Each output file should reference 1-2 most relevant example files
- Prioritize examples from the same stack

**Example:**
If generating a Payment service for .NET with CRUD operations, and examples include:
- dotnet/services/UserService.cs
- dotnet/repositories/UserRepository.cs
- dotnet/models/User.cs

Plan should include:
- PaymentService.cs → use UserService.cs as reference
- PaymentRepository.cs → use UserRepository.cs as reference
- Payment.cs → use User.cs as reference
"""),
    ("human", """Create a generation plan for the following request:

**Parsed Intent:**
Entity Name: {entity_name}
Entity Name (Plural): {entity_name_plural}
Operation Type: {operation_type}
Context: {additional_context}

**Target Stack:** {target_stack}
**Target Output Path:** {target_path}

**Available Examples:**
{examples_list}

Generate a plan that specifies:
1. Which files to create
2. The complete output path for each file
3. Which example files are relevant for each output file

**CRITICAL: Return ONLY valid JSON. No explanations, no markdown, no additional text.**

Return your plan as JSON with this structure:
{format_instructions}
""")
])


# ============================================================================
# Helper Functions
# ============================================================================

def format_examples_list(examples: Dict[str, str]) -> str:
    """
    Format the examples dictionary into a readable list for the prompt.
    Formats example files for the LLM prompt

    Args:
        examples: Dict mapping example file paths to their content

    Returns:
        Formatted string listing example files with truncated previews
    """
    if not examples:
        return "No examples available"

    lines = []
    for file_path in examples.keys():
        # Extract just the filename for brevity
        filename = file_path.split('/')[-1]
        lines.append(f"  - {file_path} ({filename})")

    return "\n".join(lines)


# ============================================================================
# Node Function
# ============================================================================

def plan_generation(state: Dict, llm: BaseChatModel) -> Dict:
    """
    Main node function that uses LLM to create generation plan
    
    Create a generation plan for which files to generate.

    This node uses the LLM to analyze the intent and examples,
    then creates a structured plan listing all files to be generated.

    Args:
        state: Current workflow state containing parsed_intent, selected_examples, etc.
        llm: The language model to use for planning

    Returns:
        Dict with generation_plan field added

    Example:
        Input state: {
            "parsed_intent": {
                "entity_name": "Payment",
                "entity_name_plural": "Payments",
                "operation_type": "crud"
            },
            "selected_examples": {
                "dotnet/services/UserService.cs": "...",
                "dotnet/models/User.cs": "..."
            },
            "target_stack": "dotnet",
            "target_path": "src"
        }

        Output: {
            "generation_plan": [
                {
                    "file_type": "service",
                    "output_path": "src/services/PaymentService.cs",
                    "relevant_examples": ["dotnet/services/UserService.cs"]
                },
                {
                    "file_type": "model",
                    "output_path": "src/models/Payment.cs",
                    "relevant_examples": ["dotnet/models/User.cs"]
                }
            ]
        }
    """
    logger.info("=" * 60)
    logger.info("Node: plan_generation")

    try:
        # Extract required info from state
        parsed_intent = state.get("parsed_intent", {})
        selected_examples = state.get("selected_examples", {})
        target_stack = state.get("target_stack", "javascript")
        target_path = state.get("target_path", "src")

        if not parsed_intent:
            raise ValueError("parsed_intent is required")

        logger.info(f"Entity: {parsed_intent.get('entity_name')}")
        logger.info(f"Operation: {parsed_intent.get('operation_type')}")
        logger.info(f"Stack: {target_stack}")
        logger.info(f"Output Path: {target_path}")
        logger.info(f"Available Examples: {len(selected_examples)}")

        # Create output parser for structured JSON response
        parser = JsonOutputParser(pydantic_object=GenerationPlanOutput)

        # Build the prompt chain
        chain = PLAN_GENERATION_PROMPT | llm | parser

        # Format examples for the prompt
        examples_list = format_examples_list(selected_examples)

        # Invoke the LLM
        try:
            result = chain.invoke({
                "entity_name": parsed_intent.get("entity_name", "Unknown"),
                "entity_name_plural": parsed_intent.get("entity_name_plural", "Unknowns"),
                "operation_type": parsed_intent.get("operation_type", "other"),
                "additional_context": parsed_intent.get("additional_context", ""),
                "target_stack": target_stack,
                "target_path": target_path,
                "examples_list": examples_list,
                "format_instructions": parser.get_format_instructions()
            })
        except Exception as parse_error:
            # If JSON parsing fails, try to extract JSON from the response manually
            logger.warning(f"JSON parsing failed, attempting manual extraction: {str(parse_error)}")

            # Get the raw response without the parser
            chain_without_parser = PLAN_GENERATION_PROMPT | llm
            raw_result = chain_without_parser.invoke({
                "entity_name": parsed_intent.get("entity_name", "Unknown"),
                "entity_name_plural": parsed_intent.get("entity_name_plural", "Unknowns"),
                "operation_type": parsed_intent.get("operation_type", "other"),
                "additional_context": parsed_intent.get("additional_context", ""),
                "target_stack": target_stack,
                "target_path": target_path,
                "examples_list": examples_list,
                "format_instructions": parser.get_format_instructions()
            })

            # Extract content from AI message
            raw_text = raw_result.content if hasattr(raw_result, 'content') else str(raw_result)

            # Try to find JSON in the response (look for { ... })
            import json
            import re

            # Find JSON object in the text
            json_match = re.search(r'\{[\s\S]*\}', raw_text)
            if json_match:
                json_str = json_match.group(0)
                result = json.loads(json_str)
                logger.info("Successfully extracted JSON from response")
            else:
                raise ValueError(f"Could not find valid JSON in response: {raw_text[:200]}")

        # Extract the files list from the result
        files = result.get("files", [])

        logger.info(f"Generated plan with {len(files)} file(s):")
        for file_plan in files:
            logger.info(f"  - {file_plan['output_path']} ({file_plan['file_type']})")

        logger.info("=" * 60)

        # Return the generation plan to add to state
        return {
            "generation_plan": files
        }

    except Exception as e:
        logger.error(f"Error generating plan: {str(e)}", exc_info=True)
        # Return error in state
        return {
            "errors": [f"Failed to generate plan: {str(e)}"]
        }


# ============================================================================
# Usage Notes
# ============================================================================

"""
How to use this node:

1. In your workflow graph:

   from workflow.nodes.plan_generation import plan_generation
   from config import get_llm

   llm = get_llm()

   # Add to graph
   graph.add_node("plan_generation", lambda state: plan_generation(state, llm))

2. The node reads from state:
   - parsed_intent (required): Output from parse_intent node
   - selected_examples (required): Output from select_examples node
   - target_stack (required): Target programming stack
   - target_path (optional): Base output path (default: "src")

3. The node adds to state:
   - generation_plan: List[Dict] with file_type, output_path, relevant_examples
   - OR errors: list if planning fails

4. Next nodes can use generation_plan:
   - generate_code iterates through the plan to generate each file
   - Each plan entry specifies which examples to use for that file

5. The LLM decides:
   - How many files to generate based on operation_type
   - Proper naming conventions based on target_stack
   - Correct folder structure following example patterns
   - Which examples are most relevant for each output file
"""
