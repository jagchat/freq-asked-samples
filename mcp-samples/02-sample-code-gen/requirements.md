## Project Generator Prototype — Consolidated Implementation Summary

---

### Goal

Build a prototype that uses **GitHub Copilot Chat as the conversational UI** to generate **project structure + boilerplate code** following established patterns, with **LangGraph as the reasoning engine**, **MCP as the bridge layer**, and a **server-side code examples library** that the LLM learns from.

---

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT SIDE                              │
│                   (Target repo / Consumer)                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│   GitHub Copilot Chat (UI)                                      │
│       - User provides prompts                                   │
│       - Displays proposed changes                               │
│       - Requires approval before applying                       │
│                                                                 │
│   .project-generator/              (Optional)                   │
│       └── conventions.json         (Project-specific overrides) │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        SERVER SIDE                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│   MCP Server (Node.js/TypeScript)                               │
│       - Exposes tools to Copilot                                │
│       - Translates requests to LangGraph                        │
│       - Returns structured file operations                      │
│                              │                                  │
│                              ▼                                  │
│   LangGraph Engine (Python)                                     │
│       - Parses user intent                                      │
│       - Selects relevant code examples                          │
│       - Generates code following patterns                       │
│       - Returns structured output                               │
│                              │                                  │
│                              ▼                                  │
│   Examples Library (Server-side)                                │
│       - Real code files (not templates)                         │
│       - Organized by language/framework                         │
│       - LLM uses as style reference                             │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

### Component Responsibilities

| Component              | Role                                               | Technology                 |
| ---------------------- | -------------------------------------------------- | -------------------------- |
| **Copilot Chat**       | Conversational UI, displays diffs, user approval   | GitHub Copilot             |
| **MCP Server**         | Exposes tools, bridges Copilot ↔ LangGraph         | Node.js / TypeScript       |
| **LangGraph Engine**   | Intent parsing, example selection, code generation | Python                     |
| **Examples Library**   | Real code samples LLM learns patterns from         | Static files (server-side) |
| **Client Conventions** | Optional project-specific overrides                | JSON (client repo)         |

---

### End-to-End Workflow

1. **User types in Copilot Chat**

   ```
   @project-generator Create a payments service with CRUD operations
   ```

2. **Copilot invokes MCP tool**

   ```json
   {
     "tool": "generate_project_structure",
     "arguments": {
       "prompt": "Create a payments service with CRUD operations",
       "stack": "nestjs"
     }
   }
   ```

3. **MCP Server calls LangGraph Engine** (via HTTP/FastAPI)

4. **LangGraph workflow executes:**

   - Parse intent → "payments service, CRUD, NestJS"
   - Select examples → Load relevant files from server-side library
   - Load client context → Fetch optional overrides from client repo
   - Generate code → LLM produces files following example patterns

5. **LangGraph returns structured result**

   ```json
   {
     "success": true,
     "files": [
       {
         "action": "create",
         "path": "src/modules/payments/payments.controller.ts",
         "content": "..."
       },
       {
         "action": "create",
         "path": "src/modules/payments/payments.service.ts",
         "content": "..."
       },
       {
         "action": "create",
         "path": "src/modules/payments/payments.repository.ts",
         "content": "..."
       },
       {
         "action": "create",
         "path": "src/modules/payments/dto/create-payment.dto.ts",
         "content": "..."
       }
     ],
     "summary": "Created 4 files for Payments service"
   }
   ```

6. **MCP returns file operations to Copilot**

7. **Copilot displays diffs → User approves → Files created**

---

### LangGraph State Model

```python
from typing import TypedDict, Optional

class GeneratorState(TypedDict):
    # Input
    user_request: str
    target_stack: str                    # "nestjs", "express", "fastapi"

    # Context
    selected_examples: dict[str, str]    # path -> content
    client_conventions: Optional[dict]   # Optional overrides from client repo

    # Processing
    parsed_intent: Optional[ParsedIntent]
    generation_plan: Optional[list[FilePlan]]

    # Output
    generated_files: list[FileOperation]
    errors: list[str]

class ParsedIntent(TypedDict):
    entity_name: str                     # "Payment"
    entity_name_plural: str              # "Payments"
    operation_type: str                  # "crud", "read-only", "custom"
    additional_context: str              # Any extra details from request

class FilePlan(TypedDict):
    file_type: str                       # "controller", "service", etc.
    output_path: str                     # Where file will be created
    relevant_examples: list[str]         # Which examples to reference

class FileOperation(TypedDict):
    action: str                          # "create" | "update"
    path: str                            # Relative file path
    content: str                         # Generated file content
```

---

### LangGraph Workflow (Nodes)

```
┌─────────────────────────┐
│     Parse Intent        │  ← Extract entity name, operation type, stack
└───────────┬─────────────┘
            ▼
┌─────────────────────────┐
│    Select Examples      │  ← Pick relevant files from server-side library
└───────────┬─────────────┘
            ▼
┌─────────────────────────┐
│  Load Client Context    │  ← Fetch optional .project-generator/conventions.json
└───────────┬─────────────┘
            ▼
┌─────────────────────────┐
│    Plan Generation      │  ← Decide which files to create, output paths
└───────────┬─────────────┘
            ▼
┌─────────────────────────┐
│    Generate Code        │  ← LLM generates each file using examples as reference
└───────────┬─────────────┘
            ▼
┌─────────────────────────┐
│    Return Output        │  ← Package as structured FileOperation[]
└─────────────────────────┘
```

---

### Server-Side Examples Library

**Location:** `langgraph-engine/examples/`

**Structure:**

```
examples/
├── javascript/
│   ├── nestjs/
│   │   ├── controllers/
│   │   │   ├── UsersController.ts
│   │   │   └── OrdersController.ts
│   │   ├── services/
│   │   │   ├── UsersService.ts
│   │   │   └── OrdersService.ts
│   │   ├── repositories/
│   │   │   ├── BaseRepository.ts
│   │   │   └── UsersRepository.ts
│   │   ├── dto/
│   │   │   ├── CreateUserDto.ts
│   │   │   └── PaginationDto.ts
│   │   ├── entities/
│   │   │   └── User.entity.ts
│   │   └── tests/
│   │       └── users.controller.spec.ts
│   │
│   └── express/
│       ├── routes/
│       ├── middleware/
│       └── ...
│
├── python/
│   └── fastapi/
│       ├── routers/
│       ├── models/
│       └── ...
│
└── shared/
    └── patterns.md              # Optional: written conventions
```

**Key principles:**

- Real code files, no placeholders or templating syntax
- Just dump working examples that represent your standards
- Organized by language/framework
- Add more examples over time to improve generation quality
- LLM uses these as **style reference**, not string replacement

---

### Example Selection Logic

```python
EXAMPLES_ROOT = Path(__file__).parent.parent / "examples"

def select_examples(intent: str, stack: str, token_budget: int = 6000) -> dict[str, str]:
    """
    Select relevant examples from server-side library based on intent.
    """

    stack_path = EXAMPLES_ROOT / "javascript" / stack

    # Map keywords to relevant folders
    relevance_map = {
        "controller": ["controllers"],
        "service": ["services", "repositories"],
        "entity": ["entities"],
        "dto": ["dto"],
        "test": ["tests"],
        "api": ["controllers", "services", "dto"],
        "crud": ["controllers", "services", "repositories", "dto"],
    }

    # Find matching folders
    folders = set()
    for keyword, dirs in relevance_map.items():
        if keyword in intent.lower():
            folders.update(dirs)

    # Default if no matches
    if not folders:
        folders = {"controllers", "services"}

    # Load files within token budget
    examples = {}
    tokens_used = 0

    for folder in folders:
        folder_path = stack_path / folder
        if not folder_path.exists():
            continue

        for file_path in sorted(folder_path.glob("*.ts"))[:2]:  # Max 2 per folder
            content = file_path.read_text()
            estimated_tokens = len(content) // 4

            if tokens_used + estimated_tokens > token_budget:
                break

            relative = str(file_path.relative_to(EXAMPLES_ROOT))
            examples[relative] = content
            tokens_used += estimated_tokens

    return examples
```

---

### Client-Side Conventions (Optional)

Clients can optionally provide `.project-generator/conventions.json` in their repo:

```json
{
  "naming": {
    "services_suffix": "Manager",
    "files": "PascalCase"
  },
  "paths": {
    "controllers": "src/api/controllers/",
    "services": "src/core/services/"
  },
  "notes": [
    "We use Manager instead of Service suffix",
    "All APIs versioned under /api/v1/"
  ]
}
```

**If not provided:** LangGraph uses examples as-is with sensible defaults.

---

### MCP Tool Schema

```json
{
  "name": "generate_project_structure",
  "description": "Generate project files following established code patterns",
  "parameters": {
    "type": "object",
    "properties": {
      "prompt": {
        "type": "string",
        "description": "User's request describing what to generate"
      },
      "stack": {
        "type": "string",
        "enum": ["nestjs", "express", "fastapi"],
        "description": "Target framework/stack",
        "default": "nestjs"
      },
      "targetPath": {
        "type": "string",
        "description": "Optional: base path for generated files"
      }
    },
    "required": ["prompt"]
  }
}
```

**Response format:**

```json
{
  "success": true,
  "files": [
    {
      "action": "create",
      "path": "src/modules/payments/payments.controller.ts",
      "content": "import { Controller, Get, Post..."
    }
  ],
  "summary": "Created 4 files for Payments service"
}
```

---

### LLM Prompt Strategy (Generate Code Node)

```
You are generating code for a {entity_name} {operation_type} in {stack}.

Here are examples showing the codebase patterns and style:

--- EXAMPLE: controllers/UsersController.ts ---
{example_content}
---

--- EXAMPLE: services/UsersService.ts ---
{example_content}
---

{if client_conventions}
Project-specific conventions:
{client_conventions_formatted}
{endif}

User request: "{user_request}"

Generate the following files, matching the patterns and style from the examples:
{generation_plan}

Return each file with its full content.
```

---

### Development Environment

| Aspect               | Choice                          | Rationale                                             |
| -------------------- | ------------------------------- | ----------------------------------------------------- |
| **Platform**         | Windows                         | Your environment                                      |
| **MCP Server**       | Node.js / TypeScript            | JavaScript priority, MCP ecosystem                    |
| **LangGraph Engine** | Python                          | Official LangGraph implementation, better LLM tooling |
| **MCP ↔ LangGraph**  | HTTP (FastAPI)                  | Clean separation, easy debugging                      |
| **LLM Provider**     | Configurable (OpenAI/Anthropic) | Start with one, make swappable                        |
| **Docker**           | Optional                        | Can containerize LangGraph service                    |

---

### Project File Structure

```
project-generator/
│
├── mcp-server/                          # Node.js MCP Server
│   ├── package.json
│   ├── tsconfig.json
│   └── src/
│       ├── index.ts                     # MCP server entry point
│       ├── tools/
│       │   └── generateStructure.ts     # Tool implementation
│       └── langgraph/
│           └── client.ts                # HTTP client for LangGraph
│
├── langgraph-engine/                    # Python LangGraph Engine
│   ├── requirements.txt
│   ├── server.py                        # FastAPI wrapper
│   ├── config.py                        # LLM configuration
│   │
│   ├── src/
│   │   ├── graph.py                     # LangGraph definition
│   │   ├── state.py                     # State TypedDict definitions
│   │   └── nodes/
│   │       ├── parse_intent.py
│   │       ├── select_examples.py
│   │       ├── load_client_context.py
│   │       ├── plan_generation.py
│   │       └── generate_code.py
│   │
│   └── examples/                        # SERVER-SIDE EXAMPLES LIBRARY
│       ├── javascript/
│       │   ├── nestjs/
│       │   │   ├── controllers/
│       │   │   │   ├── UsersController.ts
│       │   │   │   └── OrdersController.ts
│       │   │   ├── services/
│       │   │   │   └── UsersService.ts
│       │   │   ├── repositories/
│       │   │   │   └── UsersRepository.ts
│       │   │   ├── dto/
│       │   │   │   └── CreateUserDto.ts
│       │   │   └── entities/
│       │   │       └── User.entity.ts
│       │   │
│       │   └── express/
│       │       └── ...
│       │
│       └── python/
│           └── fastapi/
│               └── ...
│
└── README.md
```

---

### Prototype Scope

**Phase 1 (This Prototype):**

- MCP server with `generate_project_structure` tool
- LangGraph workflow: parse → select examples → plan → generate
- Server-side examples for one stack (NestJS)
- 5-10 example files covering controller, service, repository, DTO, entity
- Single LLM provider (OpenAI or Anthropic)
- Basic intent parsing (entity name extraction)
- Returns structured file operations

**Out of Scope (Future Phases):**

- Multiple stacks (Express, FastAPI)
- Client-side conventions loading
- Validation/linting node
- Streaming progress updates
- Caching
- Automated example extraction from repos

---

### Implementation Steps

1. **Set up MCP Server skeleton**

   - Initialize Node.js/TypeScript project
   - Register `generate_project_structure` tool
   - Create HTTP client for LangGraph

2. **Set up LangGraph Engine**

   - Initialize Python project with FastAPI
   - Define state models
   - Create basic graph with placeholder nodes

3. **Populate examples library**

   - Add 5-10 real code files for NestJS
   - Organize into controllers/, services/, etc.

4. **Implement LangGraph nodes**

   - `parse_intent`: Extract entity name from request
   - `select_examples`: Load relevant examples based on intent
   - `plan_generation`: Decide output files and paths
   - `generate_code`: LLM generates code using examples

5. **Wire end-to-end**

   - MCP tool calls LangGraph endpoint
   - LangGraph returns file operations
   - Test with mock Copilot request

6. **Test with Copilot Chat**
   - Configure Copilot to use MCP server
   - Verify file operations display correctly
   - Test approval workflow

---

### Key Implementation Decisions

| Decision          | Choice                     | Rationale                       |
| ----------------- | -------------------------- | ------------------------------- |
| Examples location | Server-side                | Centralized, maintained once    |
| Example format    | Real code, no placeholders | Simpler, LLM infers patterns    |
| Example selection | Keyword matching           | Simple, effective for prototype |
| Token management  | Budget-based loading (6K)  | Prevent context overflow        |
| MCP ↔ LangGraph   | HTTP/REST                  | Clean interface, debuggable     |
| State management  | TypedDict                  | Type safety, clear contracts    |

---

### Open Questions for Implementation

1. **LLM Provider:** OpenAI or Anthropic for LangGraph's LLM calls?
2. **First examples:** Which NestJS code samples to seed the library?
3. **Entity extraction:** Simple regex or LLM-based intent parsing?
4. **Error handling:** How should generation failures surface to user?

---

### Success Criteria for Prototype

- [ ] User can invoke `@project-generator Create a payments service` in Copilot
- [ ] MCP server receives request and calls LangGraph
- [ ] LangGraph selects relevant examples and generates code
- [ ] Generated code follows patterns from example files
- [ ] Copilot displays file diffs for user approval
- [ ] Files are created upon approval

---
