# Project Generator Prototype

A code generation system that learns from real code examples to generate new project structures.

## Architecture

This project consists of two main components:

### 1. MCP Server (Node.js - ES Modules)

- **Location**: `mcp-server/`
- **Purpose**: Acts as a bridge between GitHub Copilot Chat and the LangGraph engine
- **Technology**: Node.js with ES Modules (.mjs files)
- **Communication**: stdio (standard input/output) with GitHub Copilot
- **Responsibilities**:
  - Exposes tools that GitHub Copilot can invoke
  - Translates Copilot requests into LangGraph API calls (HTTP)
  - Returns structured file operations back to Copilot

### 2. LangGraph Engine (Python - FastAPI)

- **Location**: `langgraph-engine/`
- **Purpose**: The AI reasoning engine that generates code
- **Technology**: Python, FastAPI web server, LangGraph workflows
- **Communication**: HTTP REST API on port 8000
- **Responsibilities**:
  - Exposes `/generate` endpoint for code generation requests
  - Parses user intent from natural language requests
  - Selects relevant code examples from the library
  - Uses LLM to generate code following learned patterns
  - Returns structured file operations (JSON)

## How It Works

```
User in Copilot Chat
    ↓
    "Create a payments service with CRUD operations"
    ↓
MCP Server (Node.js)
    ↓ HTTP Request
LangGraph Engine (Python)
    ↓
    1. Parse intent → "payments service, CRUD"
    2. Select examples → Load relevant sample files
    3. Plan generation → Decide which files to create
    4. Generate code → LLM creates code using examples
    ↓ HTTP Response
MCP Server returns file operations
    ↓
Copilot shows diffs → User approves → Files created
```

## Project Structure

```
02-sample-code-gen/
├── mcp-server/                    # Node.js MCP Server
│   ├── src/
│   │   └── index.mjs             # Main MCP server (connects to Copilot)
│   ├── test-mcp.mjs              # Test script
│   ├── package.json
│   ├── run.ps1, run-watch.ps1
│
├── langgraph-engine/              # Python LangGraph Engine
│   ├── workflow/                 # LangGraph workflow components
│   │   ├── state.py              # State models (TypedDict)
│   │   ├── graph.py              # ✅ Complete workflow graph
│   │   └── nodes/                # Workflow nodes
│   │       ├── parse_intent.py   # ✅ Extract structured intent
│   │       ├── select_examples.py # ✅ Load code examples
│   │       ├── plan_generation.py # ✅ Plan file generation
│   │       └── generate_code.py   # ✅ Generate code with LLM
│   ├── server.py                 # FastAPI server
│   ├── config.py                 # LLM provider factory
│   ├── test_parse_intent.py      # Node test script
│   ├── test_select_examples.py   # Node test script
│   ├── test_plan_generation.py   # Node test script
│   ├── test_generate_code.py     # Node test script
│   ├── test_workflow.py          # End-to-end workflow test
│   ├── requirements.txt
│   └── .env.example
│
├── code-gen-source-samples/       # Code examples library
│   ├── dotnet/
│   │   ├── services/             # UserService.cs
│   │   ├── models/               # User.cs
│   │   ├── controllers/
│   │   └── repositories/
│   ├── python/
│   └── javascript/
│
├── test_langgraph_http_api.ps1    # HTTP API integration test
└── README.md
```

## Supported Technologies

The system can generate code for:

- **.NET** projects (C#, ASP.NET Core, etc.)
- **Python** projects (FastAPI, Flask, etc.)
- **JavaScript/TypeScript** projects (NestJS, Express, etc.)

## LLM Provider Support

Fully configurable support for **4 LLM providers**:

1. **OpenAI** (GPT-4, GPT-3.5-turbo)

   - Direct API connection to OpenAI
   - Requires: `OPENAI_API_KEY`

2. **Anthropic** (Claude 3.5 Sonnet, Claude Opus)

   - Direct API connection to Anthropic
   - Requires: `ANTHROPIC_API_KEY`

3. **Ollama** (Local models - llama2, codellama, mistral, etc.)

   - Runs models locally on your machine
   - No API key needed, just install Ollama

4. **LiteLLM** (Unified interface for 100+ providers)
   - Access to Azure OpenAI, AWS Bedrock, Google Gemini, Vertex AI, and more
   - Switch between cloud providers without changing code
   - Requires: `LITELLM_API_KEY` and `LITELLM_MODEL`

**Configuration**: Set `LLM_PROVIDER` in `.env` file to one of: `openai`, `anthropic`, `ollama`, or `litellm`

## Project Status

🚧 **Under Development** - Building step-by-step for learning purposes

## Getting Started

### Prerequisites

- **Node.js** (v18 or later) - for MCP server
- **Python** (3.11 or later) - for LangGraph engine
- **PowerShell** - for running setup scripts (Windows)
- **LLM API Key** - for OpenAI, Anthropic, or LiteLLM (or install Ollama for local models)

### Setup Instructions

#### 1. Setup MCP Server (Node.js)

```powershell
cd mcp-server
npm install
```

#### 2. Setup LangGraph Engine (Python)

```powershell
cd langgraph-engine
.\setup.ps1           # Creates virtual environment and installs dependencies
```

#### 3. Configure LLM Provider

```powershell
cd langgraph-engine
cp .env.example .env  # Copy the example configuration
# Edit .env and set your LLM provider and API key
```

**Example .env configuration:**

```env
# For OpenAI
LLM_PROVIDER=openai
OPENAI_API_KEY=sk-your-key-here
OPENAI_MODEL=gpt-4

# OR for Anthropic
LLM_PROVIDER=anthropic
ANTHROPIC_API_KEY=sk-ant-your-key-here
ANTHROPIC_MODEL=claude-3-5-sonnet-20241022

# OR for local Ollama
LLM_PROVIDER=ollama
OLLAMA_MODEL=codellama

# OR for LiteLLM (Azure, Bedrock, Gemini, etc.)
LLM_PROVIDER=litellm
LITELLM_API_KEY=your-key-here
LITELLM_MODEL=azure/gpt-4
```

### Running the Servers

#### Run LangGraph Engine (Python)

```powershell
cd langgraph-engine
.\run.ps1          # Normal mode
# OR
.\run-watch.ps1    # Development mode (auto-reload on file changes)
```

The FastAPI server will start on `http://127.0.0.1:8000`

- API docs available at: `http://127.0.0.1:8000/docs`

#### Run MCP Server (Node.js)

```powershell
cd mcp-server
.\run.ps1          # Normal mode
# OR
.\run-watch.ps1    # Development mode (auto-reload on file changes)
```

### Testing the Integration

Once both servers are running, you can test the HTTP connection:

```powershell
# Test the LangGraph HTTP API directly
.\test_langgraph_http_api.ps1

# Test the MCP server (simulates GitHub Copilot)
cd mcp-server
node test-mcp.mjs
```

## What We've Built So Far

✅ **Phase 1: MCP Server Foundation**

- MCP server with ES Modules (.mjs)
- `generate_project_structure` tool registered
- Tool invocation handler with proper error handling

✅ **Phase 2: LangGraph Engine Foundation**

- FastAPI web server with `/generate` endpoint
- Configuration system supporting 4 LLM providers (OpenAI, Anthropic, Ollama, LiteLLM)
- State models for LangGraph workflow
- Configurable examples path

✅ **Phase 3: MCP ↔ LangGraph Integration**

- HTTP communication between Node.js MCP server and Python FastAPI server
- Axios integration for HTTP requests
- Request/response transformation between MCP format and FastAPI format
- Comprehensive error handling (connection errors, API errors)
- Integration test scripts

✅ **Phase 4: Code Examples Library**

- Created `code-gen-source-samples/` directory structure
- Organized by stack: dotnet/, python/, javascript/
- Added sample .NET files (UserService.cs, User.cs)
- Configurable path via `EXAMPLES_PATH` in .env
- Documentation on how to add examples

✅ **Phase 5: LangGraph Workflow Nodes (Complete)**

- ✅ **parse_intent** node - Extracts structured info from natural language using LLM
  - Test script: `python test_parse_intent.py`
  - Uses Pydantic models and JsonOutputParser for type safety
- ✅ **select_examples** node - Loads relevant code examples from library
  - Test script: `python test_select_examples.py`
  - Smart filtering based on operation type (CRUD, read-only, custom)
  - Respects token limits with priority loading
- ✅ **plan_generation** node - Creates structured plan for file generation
  - Test script: `python test_plan_generation.py`
  - LLM-driven planning with stack-aware naming conventions
  - Maps relevant examples to each output file
  - Operation-type logic (CRUD vs read-only vs custom)
- ✅ **generate_code** node - Generates actual code using LLM and examples
  - Test script: `python test_generate_code.py` (requires LLM API)
  - Smart entity transformation (User → Payment in all forms)
  - Pattern learning from examples
  - Multiple file generation support
  - Production-ready code output
- ✅ **graph.py** - Wires all nodes together into complete workflow
  - `create_workflow()` - Creates and compiles LangGraph StateGraph
  - `run_workflow()` - Convenience function for one-line execution
  - Linear execution: parse_intent → select_examples → plan_generation → generate_code
  - Test script: `python test_workflow.py` (requires LLM API, tests end-to-end)

✅ **Phase 6: FastAPI Integration (Complete)**

- ✅ **server.py** - Integrated LangGraph workflow into `/generate` endpoint
  - Imports and calls `run_workflow()` from workflow.graph
  - Converts workflow results to API response format
  - Comprehensive error handling and validation
  - Human-readable summaries from parsed intent
  - Startup configuration validation
  - Complete end-to-end pipeline functional

## 🎉 Core Implementation Complete!

The LangGraph engine is now fully functional with all workflow nodes integrated into the FastAPI server. The system can:
- Parse natural language requests
- Select relevant code examples
- Plan file generation
- Generate production-ready code using LLM
- Return structured file operations via REST API

🔄 **Next Steps:**

- 🚧 End-to-end testing with real LLM code generation
- 🚧 Add more code examples to the library (Python, JavaScript)
- 🚧 Configure GitHub Copilot to use the MCP server
- 🚧 Test complete MCP → LangGraph → Copilot flow
