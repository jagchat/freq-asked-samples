"""
Parse Intent Node

This node uses an LLM to extract structured information from the user's
natural language request.  This node uses an LLM to analyze the user's request and extract:

Key Features:

- Uses Pydantic models for structured LLM output (ensures type safety)
- Uses JsonOutputParser from LangChain to parse LLM responses
- Comprehensive prompt with examples to guide the LLM
- Error handling - returns errors in state if parsing fails
- Extensive documentation and usage examples

Input (from state):
- user_request: "Create a payments service with CRUD operations"
- target_stack: "dotnet"

Output (adds to state):
- parsed_intent: {
    entity_name: "Payment",
    entity_name_plural: "Payments",
    operation_type: "crud",
    additional_context: "service with CRUD operations"
  }

Why this matters:
- Converts fuzzy human language into precise, structured data
- Helps subsequent nodes understand WHAT to generate
- Examples: "users service" → entity_name="User"
"""

from typing import Dict
import logging
from langchain_core.language_models.chat_models import BaseChatModel
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import JsonOutputParser
from pydantic import BaseModel, Field

logger = logging.getLogger(__name__)


# ============================================================================
# Pydantic Models for Structured Output
# ============================================================================

class ParsedIntentOutput(BaseModel):
    """
    Structured output from the intent parsing LLM.
    Pydantic ensures the LLM returns data in the correct format.
    """
    entity_name: str = Field(
        description="Singular form of the main entity (e.g., 'User', 'Payment', 'Product')"
    )
    entity_name_plural: str = Field(
        description="Plural form of the entity (e.g., 'Users', 'Payments', 'Products')"
    )
    operation_type: str = Field(
        description="Type of operation: 'crud' for full CRUD, 'read-only', 'custom', or 'other'"
    )
    additional_context: str = Field(
        description="Any additional context or requirements from the request"
    )


# ============================================================================
# Prompt Template
# ============================================================================

PARSE_INTENT_PROMPT = ChatPromptTemplate.from_messages([
    ("system", """You are an expert at analyzing software development requests.
Your job is to extract structured information from natural language requests.

Focus on identifying:
1. The MAIN ENTITY - the core business object (User, Payment, Product, etc.)
2. The OPERATION TYPE - what operations are needed (CRUD, read-only, custom)
3. Additional context or requirements

Guidelines:
- Entity names should be in PascalCase (singular: "User", plural: "Users")
- For operation_type, use: "crud", "read-only", "custom", or "other"
- Capture the essence, not every word

Examples:
- "Create a payments service with CRUD operations"
  → entity_name="Payment", operation_type="crud"

- "Add a users API with create and read endpoints"
  → entity_name="User", operation_type="custom"

- "Build a product catalog service"
  → entity_name="Product", operation_type="other"
"""),
    ("human", """Analyze this request and extract structured information:

Request: {user_request}
Target Stack: {target_stack}

Return your analysis as JSON with these fields:
- entity_name: Singular form (PascalCase)
- entity_name_plural: Plural form (PascalCase)
- operation_type: "crud", "read-only", "custom", or "other"
- additional_context: Brief summary of requirements

{format_instructions}
""")
])


# ============================================================================
# Node Function
# ============================================================================

def parse_intent(state: Dict, llm: BaseChatModel) -> Dict:
    """
    Parse the user's natural language request into structured intent.

    This is the first step in the workflow. It uses an LLM to understand
    what the user wants to generate.

    Args:
        state: Current workflow state containing user_request and target_stack
        llm: The language model to use for parsing

    Returns:
        Dict with parsed_intent field added

    Example:
        Input state: {
            "user_request": "Create a payments service with CRUD",
            "target_stack": "dotnet"
        }

        Output: {
            "parsed_intent": {
                "entity_name": "Payment",
                "entity_name_plural": "Payments",
                "operation_type": "crud",
                "additional_context": "service with CRUD operations"
            }
        }
    """
    logger.info("=" * 60)
    logger.info("Node: parse_intent")
    logger.info(f"Input: {state.get('user_request')}")

    try:
        # Create output parser for structured JSON response
        parser = JsonOutputParser(pydantic_object=ParsedIntentOutput)

        # Build the prompt chain
        chain = PARSE_INTENT_PROMPT | llm | parser

        # Invoke the LLM
        result = chain.invoke({
            "user_request": state["user_request"],
            "target_stack": state["target_stack"],
            "format_instructions": parser.get_format_instructions()
        })

        logger.info(f"Parsed Intent: {result}")
        logger.info("=" * 60)

        # Return the parsed intent to add to state
        return {
            "parsed_intent": result
        }

    except Exception as e:
        logger.error(f"Error parsing intent: {str(e)}", exc_info=True)
        # Return error in state
        return {
            "errors": [f"Failed to parse intent: {str(e)}"]
        }


# ============================================================================
# Usage Notes
# ============================================================================

"""
How to use this node:

1. In your workflow graph:

   from workflow.nodes.parse_intent import parse_intent
   from config import get_llm

   llm = get_llm()

   # Add to graph
   graph.add_node("parse_intent", lambda state: parse_intent(state, llm))

2. The node reads from state:
   - user_request (required)
   - target_stack (required)

3. The node adds to state:
   - parsed_intent (dict with entity_name, operation_type, etc.)
   - OR errors (list) if parsing fails

4. Next nodes can use parsed_intent to make decisions:
   - "crud" operation type → load service + repository examples
   - "read-only" → load only read operation examples
   - Entity name → generate files like "PaymentService.cs"
"""
