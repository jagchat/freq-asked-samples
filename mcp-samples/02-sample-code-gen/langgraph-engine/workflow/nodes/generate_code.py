"""
Generate Code Node

This is the final and most important node - it generates actual code using the LLM
based on the generation plan and example files.

Key Features:
- LLM-Powered Code Generation: Uses LLM to transform example code into new code
- Entity Transformation: Replaces example entity names with target entity names
- Pattern Learning: LLM learns patterns from examples and applies them to new code
- Multiple File Generation: Generates all files specified in the plan
- Complete Output: Returns FileOperation objects ready to be written to disk

Note: This node requires an active LLM connection and is the most LLM-intensive operation.  So ensure:
- Your .env file is configured with API keys
- You have API credits available
- You're using an appropriate model (GPT-4, Claude, etc.)

Input (from state):
- generation_plan: [
    {
        "file_type": "service",
        "output_path": "src/services/PaymentService.cs",
        "relevant_examples": ["dotnet/services/UserService.cs"]
    },
    ...
  ]
- selected_examples: {
    "dotnet/services/UserService.cs": "public class UserService { ... }",
    ...
  }
- parsed_intent: {
    entity_name: "Payment",
    entity_name_plural: "Payments",
    ...
  }

Output (adds to state):
- generated_files: [
    {
        "action": "create",
        "path": "src/services/PaymentService.cs",
        "content": "public class PaymentService { /* generated code */ }"
    },
    ...
  ]
"""

from typing import Dict, List
import logging
from langchain_core.language_models.chat_models import BaseChatModel
from langchain_core.prompts import ChatPromptTemplate

logger = logging.getLogger(__name__)


# ============================================================================
# Helper Functions for Token Tracking
# ============================================================================

def extract_token_usage(response, current_usage: Dict[str, int]) -> Dict[str, int]:
    """
    Extract token usage from LLM response and accumulate with current usage.

    Args:
        response: The raw response from the LLM (AIMessage object)
        current_usage: Current accumulated token usage dict

    Returns:
        Dict with input_tokens, output_tokens, and total_tokens
    """
    # Get existing token usage
    existing_input = current_usage.get("input_tokens", 0)
    existing_output = current_usage.get("output_tokens", 0)
    existing_total = current_usage.get("total_tokens", 0)

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

    # Accumulate and return
    return {
        "input_tokens": existing_input + new_input,
        "output_tokens": existing_output + new_output,
        "total_tokens": existing_total + new_input + new_output
    }


# ============================================================================
# Prompt Template
# ============================================================================

GENERATE_CODE_PROMPT = ChatPromptTemplate.from_messages([
    ("system", """You are an expert software developer who generates high-quality code based on examples.

Your job is to:
1. Study the provided example code carefully
2. Learn the patterns, structure, and conventions
3. Generate NEW code for the target entity that follows the same patterns

**CRITICAL RULES:**

1. **Entity Name Transformation:**
   - Find all occurrences of the example entity name in the example code
   - Replace them with the target entity name in all forms:
     - Singular form (User → Payment)
     - Plural form (Users → Payments)
     - In class names (UserService → PaymentService)
     - In variable names (user → payment, users → payments)
     - In comments and documentation
   - Preserve the EXACT casing pattern (PascalCase stays PascalCase, camelCase stays camelCase)

2. **Maintain Code Structure:**
   - Keep the same file structure and organization
   - Use the same patterns for methods, properties, fields
   - Follow the same error handling patterns
   - Keep the same imports/using statements (adjust as needed)

3. **Preserve Quality:**
   - Maintain the same level of documentation
   - Keep the same code style and formatting
   - Preserve validation logic patterns
   - Keep any interfaces or abstract classes

4. **Adapt Thoughtfully:**
   - If the example has domain-specific logic (e.g., "email validation" for User),
     adapt it to make sense for the new entity (e.g., "amount validation" for Payment)
   - Keep business logic patterns but adjust specifics to fit the new entity

5. **Complete Code Only:**
   - Return ONLY the complete, working code
   - NO explanations, NO markdown code blocks, NO comments about the transformation
   - Just pure code ready to be saved to a file

**Example Transformation:**

If example has:
```csharp
public class UserService
{{{{
    private readonly IUserRepository _userRepository;

    public async Task<User> GetByIdAsync(int id)
    {{{{
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {{{{
            throw new KeyNotFoundException($"User with ID {{{{id}}}} not found");
        }}}}
        return user;
    }}}}
}}}}
```

And target entity is "Payment", generate:
```csharp
public class PaymentService
{{{{
    private readonly IPaymentRepository _paymentRepository;

    public async Task<Payment> GetByIdAsync(int id)
    {{{{
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
        {{{{
            throw new KeyNotFoundException($"Payment with ID {{{{id}}}} not found");
        }}}}
        return payment;
    }}}}
}}}}
```
"""),
    ("human", """**CRITICAL: Return ONLY the raw code. No explanations. No markdown code blocks like ```csharp or ```python. Just the pure source code that can be saved directly to a file.**

Generate code for this file:

**Target Entity:**
- Singular: {entity_name}
- Plural: {entity_name_plural}

**File Information:**
- Type: {file_type}
- Output Path: {output_path}

**Example Code to Learn From:**

{example_code}

---

Generate the complete code for the new file. Remember:
- Transform ALL occurrences of the example entity to the target entity
- Preserve ALL patterns and structure from the example
- Return ONLY the raw code (no explanations, no markdown blocks)
- Make it production-ready

IMPORTANT: Output raw code only, starting immediately with the code itself (e.g., "using System;" or "public class" for C#).
""")
])


# ============================================================================
# Helper Functions
# ============================================================================

def extract_entity_name_from_example(example_path: str) -> str:
    """
    Extract the entity name from an example file path.

    For example:
    - "dotnet/services/UserService.cs" → "User"
    - "python/models/user.py" → "User"
    - "javascript/models/product.model.js" → "Product"

    Args:
        example_path: Path to the example file

    Returns:
        The entity name in PascalCase (e.g., "User", "Product")
    """
    # Get the filename without extension
    filename = example_path.split('/')[-1]
    filename_without_ext = filename.rsplit('.', 1)[0]

    # Remove common suffixes
    for suffix in ['Service', 'Repository', 'Controller', 'Model', '.model', '_service', '_repository', '_controller']:
        if filename_without_ext.endswith(suffix):
            filename_without_ext = filename_without_ext[:-len(suffix)]
            break

    # Convert to PascalCase
    entity_name = filename_without_ext.replace('_', ' ').title().replace(' ', '')

    return entity_name


def format_example_code(examples: Dict[str, str], relevant_example_paths: List[str]) -> str:
    """
    Format the relevant example code for the prompt.

    Args:
        examples: All available examples (path → content)
        relevant_example_paths: List of paths to include in this prompt

    Returns:
        Formatted string with example code
    """
    formatted = []

    for example_path in relevant_example_paths:
        content = examples.get(example_path, "")
        if content:
            formatted.append(f"File: {example_path}")
            formatted.append("```")
            formatted.append(content)
            formatted.append("```")
            formatted.append("")

    return "\n".join(formatted)


# ============================================================================
# Node Function
# ============================================================================

def generate_code(state: Dict, llm: BaseChatModel) -> Dict:
    """
    Generate actual code for all files in the generation plan.

    This is the final node that produces the actual code output.
    It iterates through each file in the plan and uses the LLM
    to generate code based on the relevant examples.

    Features:
    - Iterates through each file in the generation plan
    - Uses LLM to transform example code to new entity code
    - Cleans up markdown code blocks if LLM includes them
    - Continues generating other files even if one fails
    - Returns FileOperation objects ready for disk write
    
    Args:
        state: Current workflow state containing generation_plan, selected_examples, parsed_intent
        llm: The language model to use for code generation

    Returns:
        Dict with generated_files field added

    Example:
        Input state: {
            "generation_plan": [
                {
                    "file_type": "service",
                    "output_path": "src/services/PaymentService.cs",
                    "relevant_examples": ["dotnet/services/UserService.cs"]
                }
            ],
            "selected_examples": {
                "dotnet/services/UserService.cs": "public class UserService { ... }"
            },
            "parsed_intent": {
                "entity_name": "Payment",
                "entity_name_plural": "Payments"
            }
        }

        Output: {
            "generated_files": [
                {
                    "action": "create",
                    "path": "src/services/PaymentService.cs",
                    "content": "public class PaymentService { /* complete generated code */ }"
                }
            ]
        }
    """
    logger.info("=" * 60)
    logger.info("Node: generate_code")

    try:
        # Extract required info from state
        generation_plan = state.get("generation_plan", [])
        selected_examples = state.get("selected_examples", {})
        parsed_intent = state.get("parsed_intent", {})

        entity_name = parsed_intent.get("entity_name", "Unknown")
        entity_name_plural = parsed_intent.get("entity_name_plural", "Unknowns")

        if not generation_plan:
            raise ValueError("generation_plan is required and cannot be empty")

        logger.info(f"Generating {len(generation_plan)} file(s)...")
        logger.info(f"Entity: {entity_name} (plural: {entity_name_plural})")

        # Initialize token tracking (get existing usage from state, handle None case)
        token_usage = state.get("token_usage") or {}
        if not token_usage:
            token_usage = {"input_tokens": 0, "output_tokens": 0, "total_tokens": 0}

        # Build the prompt chain (we'll invoke it for each file)
        chain = GENERATE_CODE_PROMPT | llm

        # Generate code for each file in the plan
        generated_files = []

        for idx, file_plan in enumerate(generation_plan, 1):
            file_type = file_plan.get("file_type", "unknown")
            output_path = file_plan.get("output_path", "")
            relevant_examples = file_plan.get("relevant_examples", [])

            logger.info(f"[{idx}/{len(generation_plan)}] Generating: {output_path}")
            logger.info(f"  Type: {file_type}")
            logger.info(f"  Using examples: {', '.join(relevant_examples)}")

            # Format the example code
            example_code = format_example_code(selected_examples, relevant_examples)

            if not example_code.strip():
                logger.warning(f"  ⚠ No example code found for {output_path}, skipping")
                continue

            # Invoke the LLM to generate code
            try:
                result = chain.invoke({
                    "entity_name": entity_name,
                    "entity_name_plural": entity_name_plural,
                    "file_type": file_type,
                    "output_path": output_path,
                    "example_code": example_code
                })

                # Extract token usage from this generation
                token_usage = extract_token_usage(result, token_usage)

                # Extract the generated code
                # The result is an AIMessage object, get its content
                generated_content = result.content if hasattr(result, 'content') else str(result)

                # Clean up the content (remove markdown code blocks if present)
                generated_content = generated_content.strip()
                if generated_content.startswith("```"):
                    # Remove markdown code blocks
                    lines = generated_content.split('\n')
                    # Remove first line (```)
                    lines = lines[1:]
                    # Remove last line if it's ```)
                    if lines and lines[-1].strip() == "```":
                        lines = lines[:-1]
                    generated_content = '\n'.join(lines)

                logger.info(f"  ✓ Generated {len(generated_content)} characters")

                # Add to generated files
                generated_files.append({
                    "action": "create",
                    "path": output_path,
                    "content": generated_content
                })

            except Exception as e:
                logger.error(f"  ✗ Failed to generate {output_path}: {str(e)}")
                # Continue with other files even if one fails
                continue

        logger.info(f"Successfully generated {len(generated_files)} file(s)")

        # Log token usage
        if token_usage and token_usage.get('total_tokens', 0) > 0:
            logger.info(f"Token Usage: {token_usage['total_tokens']} tokens (input: {token_usage['input_tokens']}, output: {token_usage['output_tokens']})")

        logger.info("=" * 60)

        # Return the generated files and token usage to add to state
        return {
            "generated_files": generated_files,
            "token_usage": token_usage
        }

    except Exception as e:
        logger.error(f"Error generating code: {str(e)}", exc_info=True)
        # Return error in state
        return {
            "errors": [f"Failed to generate code: {str(e)}"]
        }


# ============================================================================
# Usage Notes
# ============================================================================

"""
How to use this node:

1. In your workflow graph:

   from workflow.nodes.generate_code import generate_code
   from config import get_llm

   llm = get_llm()

   # Add to graph
   graph.add_node("generate_code", lambda state: generate_code(state, llm))

2. The node reads from state:
   - generation_plan (required): Output from plan_generation node
   - selected_examples (required): Output from select_examples node
   - parsed_intent (required): Output from parse_intent node

3. The node adds to state:
   - generated_files: List[Dict] with action="create", path, and content
   - OR errors: list if generation fails

4. The generated_files format matches the expected server response:
   - Each file has: action, path, content
   - Ready to be written to disk by the server
   - Can be returned directly to the MCP server

5. Performance notes:
   - This is the most LLM-intensive node (one API call per file)
   - Generation time depends on file complexity and LLM speed
   - Consider using a faster model (like gpt-3.5-turbo) for testing
   - Production should use a more capable model (gpt-4, claude-3.5-sonnet)

6. Error handling:
   - If one file fails, others will still be generated
   - Failed files are logged but don't stop the entire process
   - Check the errors field in the result for any issues
"""
