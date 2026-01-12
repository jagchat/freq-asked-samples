"""
FastAPI Server for LangGraph Code Generation Engine

This server exposes HTTP endpoints that the MCP server can call to generate code.
It acts as a REST API wrapper around the LangGraph workflow.
"""

from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field
from typing import List, Optional, Literal
import logging
from pathlib import Path
from workflow.graph import run_workflow
from config import get_config, print_config_summary

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# ============================================================================
# FastAPI Application
# ============================================================================

app = FastAPI(
    title="Project Generator - LangGraph Engine",
    description="Code generation engine that learns from examples",
    version="0.1.0"
)

# Enable CORS so the MCP server (Node.js) can call this API
# Allows the MCP server (Node.js on a different process) to call this API
# Without this, browsers/Node would block the requests
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # In production, restrict this to specific origins
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ============================================================================
# Request/Response Models
# ============================================================================

class FileOperation(BaseModel):
    """
    Represents a file operation (create or update).
    Represents one file to create or update
    This is what the LangGraph workflow will return.
    """
    action: Literal["create", "update"] = Field(
        description="Type of operation: create new file or update existing"
    )
    path: str = Field(
        description="Relative file path (e.g., 'src/services/PaymentService.cs')"
    )
    content: str = Field(
        description="Full content of the file"
    )


class GenerateRequest(BaseModel):
    """
    Request to generate project structure.
    What the MCP server sends to us
    Matches the tool parameters from the MCP server
    """
    prompt: str = Field(
        description="User's natural language request describing what to generate",
        example="Create a payments service with CRUD operations"
    )
    stack: Literal["dotnet", "python", "javascript"] = Field(
        default="javascript",
        description="Target technology stack"
    )
    target_path: Optional[str] = Field(
        default="src",
        description="Base path where files should be generated"
    )


class GenerateResponse(BaseModel):
    """
    Response containing generated files.
    This is what we send back to the MCP server.
    Contains all the generated files
    """
    success: bool = Field(
        description="Whether generation succeeded"
    )
    files: List[FileOperation] = Field(
        description="List of file operations to perform"
    )
    summary: str = Field(
        description="Human-readable summary of what was generated"
    )
    errors: Optional[List[str]] = Field(
        default=None,
        description="List of errors if generation failed"
    )


# ============================================================================
# Endpoints
# ============================================================================

@app.get("/")
async def root():
    """
    Health check endpoint.
    Returns basic information about the API.
    """
    return {
        "name": "LangGraph Code Generation Engine",
        "version": "0.1.0",
        "status": "running",
        "endpoints": {
            "generate": "/generate",
            "health": "/health",
            "docs": "/docs"
        }
    }


@app.get("/health")
async def health():
    """
    Health check endpoint for monitoring.
    """
    return {
        "status": "healthy",
        "service": "langgraph-engine"
    }


@app.post("/generate", response_model=GenerateResponse)
async def generate_code(request: GenerateRequest):
    """
    Main endpoint for code generation.

    This endpoint:
    - Receives a generation request (GenerateRequest) from the MCP server
    - Validates the request (Pydantic does this automatically)
    - Logs the request
    - Invokes the LangGraph workflow 
    - Returns structured file operations (GenerateResponse) back to the MCP server

    Args:
        request: GenerateRequest containing prompt, stack, and target_path

    Returns:
        GenerateResponse with list of files to create/update
    """
    logger.info(f"Received generation request: {request.prompt}")
    logger.info(f"Stack: {request.stack}, Target path: {request.target_path}")

    try:
        # Invoke the LangGraph workflow
        logger.info("Invoking LangGraph workflow...")
        workflow_result = run_workflow(
            user_request=request.prompt,
            target_stack=request.stack,
            target_path=request.target_path
        )

        # Extract generated files from workflow result
        generated_files = workflow_result.get("generated_files", [])
        errors = workflow_result.get("errors", [])

        # Check if workflow succeeded
        if errors:
            logger.error(f"Workflow completed with errors: {errors}")
            return GenerateResponse(
                success=False,
                files=[],
                summary=f"Generation failed with {len(errors)} error(s)",
                errors=errors
            )

        if not generated_files:
            logger.warning("Workflow completed but generated no files")
            return GenerateResponse(
                success=False,
                files=[],
                summary="No files were generated",
                errors=["Workflow completed but produced no output files"]
            )

        # Convert workflow files to API response format
        file_operations = [
            FileOperation(
                action=file_info["action"],
                path=file_info["path"],
                content=file_info["content"]
            )
            for file_info in generated_files
        ]

        # Create human-readable summary
        parsed_intent = workflow_result.get("parsed_intent", {})
        entity_name = parsed_intent.get("entity_name", "entity")
        operation_type = parsed_intent.get("operation_type", "service")

        summary = (
            f"Generated {len(file_operations)} file(s) for {entity_name} {operation_type}. "
            f"Files: {', '.join([f['path'].split('/')[-1] for f in generated_files[:3]])}"
            f"{'...' if len(generated_files) > 3 else ''}"
        )

        response = GenerateResponse(
            success=True,
            files=file_operations,
            summary=summary,
            errors=None
        )

        logger.info(f"✓ Successfully generated {len(response.files)} files")
        return response

    except Exception as e:
        logger.error(f"Error during generation: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail=f"Code generation failed: {str(e)}"
        )


# ============================================================================
# Startup/Shutdown Events
# ============================================================================

@app.on_event("startup")
async def startup_event():
    """
    Runs when the server starts.
    Use this to initialize resources (load config, connect to services, etc.)
    """
    logger.info("="* 60)
    logger.info("LangGraph Code Generation Engine Starting...")
    logger.info("="* 60)

    # Load and validate configuration
    try:
        config = get_config()
        logger.info("Configuration loaded successfully")
        print_config_summary()

        # Validate that examples directory exists
        examples_path = Path(config.examples_path)
        if not examples_path.exists():
            logger.warning(f"Examples directory not found: {examples_path}")
            logger.warning("Please create the examples directory and add sample code")
        else:
            logger.info(f"✓ Examples directory found: {examples_path}")

        logger.info("="* 60)
        logger.info("✓ Server ready to accept requests")
        logger.info("="* 60)

    except Exception as e:
        logger.error(f"Failed to initialize server: {str(e)}", exc_info=True)
        raise


@app.on_event("shutdown")
async def shutdown_event():
    """
    Runs when the server shuts down.
    Use this to clean up resources.
    """
    logger.info("LangGraph Engine shutting down...")


# ============================================================================
# Development Entry Point
# ============================================================================

if __name__ == "__main__":
    import uvicorn

    # This allows running the server with: python server.py
    # In production, use: uvicorn server:app
    uvicorn.run(
        "server:app",
        host="127.0.0.1",
        port=8000,
        reload=True,  # Auto-reload on code changes
        log_level="info"
    )


"""
API Workflow:
-------------
1. MCP Server (Node.js) receives request from GitHub Copilot
   └─> POST http://127.0.0.1:8000/generate
       Body: {
         "prompt": "Create a payments service with CRUD",
         "stack": "dotnet",
         "target_path": "src"
       }

2. FastAPI server receives the request
   └─> Validates request using Pydantic models
   └─> [Future] Invokes LangGraph workflow
   └─> Returns structured response

3. MCP Server receives response
   Body: {
     "success": true,
     "files": [
       {
         "action": "create",
         "path": "src/PaymentService.cs",
         "content": "... generated code ..."
       }
     ],
     "summary": "Created 4 files for Payment service"
   }

4. MCP Server formats response for GitHub Copilot
   └─> Copilot displays file diffs
   └─> User approves
   └─> Files are created
"""
