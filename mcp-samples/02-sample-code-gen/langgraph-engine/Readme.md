# LangGraph Engine

An AI-powered code generation engine that learns from real code examples to generate new project structures. This engine analyzes natural language requests, studies patterns from your example code library, and generates complete, production-ready code following your established conventions.

**How it works:** You provide a natural language request (e.g., "Create a payments service with CRUD operations"), and the LangGraph workflow intelligently selects relevant examples from your code library, analyzes their patterns, and generates new code that follows the same structure, naming conventions, and best practices.

---

## LangChain:

- Framework for building LLM applications
- Provides tools to work with different AI providers
- Handles prompts, responses, and chains of operations

## LangChain-Core:

- Core abstractions used by LangChain
- Required by both LangChain and LangGraph

## LangGraph:

- The core library for building AI workflows as state machines
- Think of it as a flow chart for AI operations:
  - Parse intent → Select examples → Plan → Generate code
- Each step is a "node", and data flows between them

## langchain-openai:

- Connects to OpenAI's API
- Use models like: gpt-4, gpt-3.5-turbo
- Requires OpenAI API key

## langchain-anthropic:

- Connects to Anthropic's API
- Use models like: claude-3-5-sonnet-20241022, claude-3-opus-20240229
- Requires Anthropic API key

## langchain-ollama:

- Connects to Ollama running locally on your machine
- Use models like: llama2, codellama, mistral
- No API key needed, runs on your computer

## LiteLLM (via langchain-community):

- **Unified interface for 100+ LLM providers**
- One API to access: Azure OpenAI, AWS Bedrock, Google Gemini, Vertex AI, and more
- **Why it's useful:**
  - Switch between cloud providers without changing code
  - Use enterprise-hosted models (Azure, AWS, GCP)
  - Fallback to different providers if one fails
- **Example models:**
  - `gpt-4` → OpenAI (direct)
  - `azure/gpt-4` → Azure OpenAI
  - `bedrock/anthropic.claude-3-5-sonnet-20240620-v1:0` → AWS Bedrock
  - `gemini/gemini-pro` → Google Gemini
  - `ollama/codellama` → Local Ollama
- **Configuration:** Set `LLM_PROVIDER=litellm` in .env and configure model name

## .ENV Configuration

- **LLM_PROVIDER**: Which LLM to use (openai, anthropic, ollama, litellm)
- **MAX_EXAMPLES_TOKENS**: How many example code files to load (to fit in LLM context)
- **LLM_TEMPERATURE**: Creativity vs consistency (0.2 is good for code)
- **EXAMPLES_PATH**: Path to code examples library (default: `../code-gen-source-samples`)

## How File Generation Actually Works

### Your Request → Multiple Files with Different Types

- When you ask: _"Create a payment service in .NET"_

The LangGraph workflow will:

- **Analyze the request**: "payment service" + "dotnet stack"
- **Select relevant examples** from the examples library:
  - Example .cs service files
  - Example .csproj files
  - Example appsettings.json
  - Example folder structures
- **Generate a complete project** with proper structure:

src/
├── PaymentService/
│ ├── PaymentService.csproj ← Project file
│ ├── appsettings.json ← Configuration
│ ├── Controllers/
│ │ └── PaymentController.cs ← Controller
│ ├── Services/
│ │ └── PaymentService.cs ← Business logic
│ ├── Models/
│ │ └── Payment.cs ← Data model
│ └── Program.cs ← Entry point

### The LLM Determines Everything

- **File paths**: Complete paths with correct extensions
- **Folder structure**: Proper organization
- **Multiple file types**: .cs, .csproj, .json, .md, etc.
- **Content**: Real code based on learned patterns from examples

### The "stack" Parameter's Real Purpose

The `stack` parameter tells LangGraph:

- **Which examples folder to use**: `code-gen-source-samples/dotnet/`, `code-gen-source-samples/python/`, etc.
- **What patterns to follow**: .NET conventions vs Python conventions vs Node.js conventions
- **What project structure to create**: .NET solution vs Python package vs npm package

It's **NOT** about file extensions - it's about **which coding patterns and project structures to learn from**.

## Project Structure

```
langgraph-engine/
├── config.py              # Configuration & LLM provider factory
├── server.py              # FastAPI server with /generate endpoint
├── workflow/              # LangGraph workflow components
│   ├── state.py          # State models (TypedDict definitions)
│   ├── nodes/            # Workflow nodes
│   │   ├── parse_intent.py      # ✅ IMPLEMENTED: Extract structured intent
│   │   ├── select_examples.py   # ✅ IMPLEMENTED: Load relevant examples
│   │   ├── plan_generation.py   # ✅ IMPLEMENTED: Decide which files to create
│   │   └── generate_code.py     # ✅ IMPLEMENTED: Generate actual code
│   └── graph.py          # ✅ IMPLEMENTED: Wire all nodes together
├── test_parse_intent.py  # Test script for parse_intent node
├── test_select_examples.py # Test script for select_examples node
├── test_plan_generation.py # Test script for plan_generation node
├── test_generate_code.py # Test script for generate_code node
├── test_workflow.py     # Test script for complete end-to-end workflow
└── requirements.txt      # Python dependencies
```

## Implementation Status

### ✅ Completed Nodes

**1. `parse_intent` Node** ([workflow/nodes/parse_intent.py](workflow/nodes/parse_intent.py))

- Uses LLM to extract structured information from natural language
- Outputs: entity_name, entity_name_plural, operation_type, additional_context
- Includes comprehensive prompts and Pydantic models for type safety
- Test with: `python test_parse_intent.py`

**2. `select_examples` Node** ([workflow/nodes/select_examples.py](workflow/nodes/select_examples.py))

- Loads relevant code examples from `code-gen-source-samples` directory
- Filters examples based on operation type (CRUD → services, repositories, models, controllers)
- Respects `MAX_EXAMPLES_TOKENS` limit to fit in LLM context
- Uses priority loading: services → repositories → models → controllers
- Test with: `python test_select_examples.py`

**3. `plan_generation` Node** ([workflow/nodes/plan_generation.py](workflow/nodes/plan_generation.py))

- Uses LLM to create structured plan for which files to generate
- Stack-aware naming conventions (.NET: PascalCase, Python: snake_case, JS: camelCase)
- Operation-type logic (CRUD → service+repository+model, read-only → service+model)
- Maps relevant example files to each output file
- Returns list of FilePlan objects with output_path and relevant_examples
- Test with: `python test_plan_generation.py`

**4. `generate_code` Node** ([workflow/nodes/generate_code.py](workflow/nodes/generate_code.py))

- Uses LLM to transform example code into new entity code
- Smart entity name replacement (User → Payment, user → payment, users → payments)
- Preserves code structure, patterns, and conventions from examples
- Generates multiple files based on the generation plan
- Returns FileOperation objects ready to be written to disk
- Test with: `python test_generate_code.py` (requires LLM API access)

**5. `graph.py`** ([workflow/graph.py](workflow/graph.py))

- Wires all 4 nodes together into a complete LangGraph StateGraph workflow
- `create_workflow()` function: Initializes LLM, creates StateGraph, adds nodes, defines edges, compiles workflow
- `run_workflow()` convenience function: One-line execution from request to generated files
- Linear execution path: START → parse_intent → select_examples → plan_generation → generate_code → END
- Automatic state merging: LangGraph accumulates data as it flows through nodes
- Ready for FastAPI integration
- Test with: `python test_workflow.py` (requires LLM API access)

**6. FastAPI Integration** ([server.py](server.py))

- Integrated LangGraph workflow into `/generate` POST endpoint
- Calls `run_workflow()` with request parameters (prompt, stack, target_path)
- Converts workflow result to API response format (GenerateResponse)
- Comprehensive error handling for workflow failures
- Human-readable summary generation from parsed intent
- Startup validation: Checks configuration and examples directory
- Complete end-to-end pipeline: API request → LangGraph workflow → Generated files

### ✅ Implementation Complete!

All workflow nodes and server integration are complete. The system is ready for end-to-end testing with real LLM code generation.

## What is State in LangGraph?

Think of **state** as a **data bucket** that gets passed from node to node in the workflow:

Initial State After Node 1 After Node 2 Final State
┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│ user_request│ Node 1 │ user_request│ Node 2 │ user_request│ Node 3 │ user_request│
│ target_stack│ ──────> │ target_stack│ ────> │ target_stack│ ────> │ target_stack│
│ │ parse │ parsed_intent│ select│ parsed_intent│generate│ parsed_intent│
│ │ intent │ ✓ ADDED │examples│ examples ✓ │ code │ files ✓ │
└─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘
Each node:

- **Reads** from the state
- **Adds** new information
- **Passes** the updated state to the next node

## How State Flows - Real Example

Let's trace a real request through the workflow:

**User Request:** "Create a payments service with CRUD operations"

**Step 1: Initial State**

```
{
"user_request": "Create a payments service with CRUD operations",
"target_stack": "dotnet",
"target_path": "src",
"selected_examples": None,
"parsed_intent": None,
"generation_plan": None,
"generated_files": [],
"errors": []
}
```

**Step 2: After `parse_intent` node**

```
{ # Input (unchanged)
"user_request": "Create a payments service with CRUD operations",
"target_stack": "dotnet",
"target_path": "src",

    # NEW: Parsed intent added
    "parsed_intent": {
        "entity_name": "Payment",
        "entity_name_plural": "Payments",
        "operation_type": "crud",
        "additional_context": "service with CRUD operations"
    },

    # Rest still None/empty
    "selected_examples": None,
    "generation_plan": None,
    "generated_files": [],
    "errors": []

}
```

**Step 3: After `select_examples` node**

```
{ # Previous data preserved
"user_request": "...",
"target_stack": "dotnet",
"parsed_intent": { "entity_name": "Payment", ... },

    # NEW: Examples loaded
    "selected_examples": {
        "dotnet/services/UserService.cs": "public class UserService { ... }",
        "dotnet/repositories/UserRepository.cs": "public class UserRepository { ... }"
    },

    # Rest still None/empty
    "generation_plan": None,
    "generated_files": [],
    "errors": []

}
```

**Step 4: After `plan_generation` node**

```
{ # All previous data preserved
"user_request": "...",
"parsed_intent": { ... },
"selected_examples": { ... },

    # NEW: Plan created
    "generation_plan": [
        {
            "file_type": "service",
            "output_path": "src/services/PaymentService.cs",
            "relevant_examples": ["dotnet/services/UserService.cs"]
        },
        {
            "file_type": "repository",
            "output_path": "src/repositories/PaymentRepository.cs",
            "relevant_examples": ["dotnet/repositories/UserRepository.cs"]
        }
    ],

    # Output still empty
    "generated_files": [],
    "errors": []

}
```

**Step 5: After `generate_code` node (FINAL)**

```
{ # All previous data preserved
"user_request": "...",
"parsed_intent": { ... },
"selected_examples": { ... },
"generation_plan": [ ... ],

    # NEW: Files generated!
    "generated_files": [
        {
            "action": "create",
            "path": "src/services/PaymentService.cs",
            "content": "public class PaymentService { /* full generated code */ }"
        },
        {
            "action": "create",
            "path": "src/repositories/PaymentRepository.cs",
            "content": "public class PaymentRepository { /* full generated code */ }"
        }
    ],
    "errors": []

}
```
