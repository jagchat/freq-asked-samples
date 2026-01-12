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
# Helper Functions
# ============================================================================

def extract_token_usage(response, state: Dict) -> Dict[str, int]:
    """
    Extract token usage from LLM response and accumulate with existing usage.

    Args:
        response: The raw response from the LLM (AIMessage object)
        state: Current workflow state (to get existing token_usage)

    Returns:
        Dict with input_tokens, output_tokens, and total_tokens
    """
    # Get existing token usage from state (handle None case)
    existing_usage = state.get("token_usage") or {}
    existing_input = existing_usage.get("input_tokens", 0)
    existing_output = existing_usage.get("output_tokens", 0)
    existing_total = existing_usage.get("total_tokens", 0)

    # Try to extract token usage from response
    # Different providers return usage in different formats
    new_input = 0
    new_output = 0

    # Try usage_metadata (newer LangChain format)
    if hasattr(response, 'usage_metadata') and response.usage_metadata:
        usage = response.usage_metadata
        new_input = usage.get('input_tokens', 0)
        new_output = usage.get('output_tokens', 0)
    # Try response_metadata (older format)
    elif hasattr(response, 'response_metadata') and response.response_metadata:
        metadata = response.response_metadata
        # OpenAI format
        if 'token_usage' in metadata:
            token_usage = metadata['token_usage']
            new_input = token_usage.get('prompt_tokens', 0)
            new_output = token_usage.get('completion_tokens', 0)
        # Anthropic format
        elif 'usage' in metadata:
            usage = metadata['usage']
            new_input = usage.get('input_tokens', 0)
            new_output = usage.get('output_tokens', 0)

    # If we found token usage, accumulate and return
    if new_input > 0 or new_output > 0:
        return {
            "input_tokens": existing_input + new_input,
            "output_tokens": existing_output + new_output,
            "total_tokens": existing_total + new_input + new_output
        }

    # If no token usage found, return existing or None
    return existing_usage if existing_usage else None


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
    ("human", """**CRITICAL: You must return ONLY valid JSON. No explanations before or after. No markdown code blocks. Just raw JSON starting with {{ and ending with }}.**

Analyze this request and extract structured information:

Request: {user_request}
Target Stack: {target_stack}

Return your analysis as JSON with these exact fields:
- entity_name: Singular form (PascalCase)
- entity_name_plural: Plural form (PascalCase)
- operation_type: "crud", "read-only", "custom", or "other"
- additional_context: Brief summary of requirements

{format_instructions}

Remember: Return ONLY the JSON object, nothing else.
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

        # Build the prompt chain (without parser to get raw response with metadata)
        chain_without_parser = PARSE_INTENT_PROMPT | llm

        # Invoke the LLM
        raw_response = chain_without_parser.invoke({
            "user_request": state["user_request"],
            "target_stack": state["target_stack"],
            "format_instructions": parser.get_format_instructions()
        })

        # Parse the content
        result = parser.parse(raw_response.content)

        logger.info(f"Parsed Intent: {result}")

        # Extract token usage from response
        token_usage = extract_token_usage(raw_response, state)
        if token_usage:
            logger.info(f"Token Usage: {token_usage['total_tokens']} tokens (input: {token_usage['input_tokens']}, output: {token_usage['output_tokens']})")

        logger.info("=" * 60)

        # Return the parsed intent and token usage to add to state
        return_dict = {
            "parsed_intent": result
        }
        if token_usage:
            return_dict["token_usage"] = token_usage

        return return_dict

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
