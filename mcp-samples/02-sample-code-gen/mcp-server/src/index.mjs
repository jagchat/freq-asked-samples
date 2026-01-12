#!/usr/bin/env node

/**
 * MCP Server Entry Point
 *
 * This file creates an MCP (Model Context Protocol) server that GitHub Copilot
 * can communicate with. It exposes tools that Copilot can invoke to generate
 * code based on examples.
 */

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from '@modelcontextprotocol/sdk/types.js';
import axios from 'axios';

/**
 * Create the MCP Server instance
 *
 * The server has:
 * - name: Identifies this server to clients (like GitHub Copilot)
 *         Also, declares what capabilities it has (tools it provides)
 *         think like opening a shop and putting up a sign saying "We provide code gen tools here"
 * - version: Tracks the server version
 */
const server = new Server(
  {
    name: 'project-generator-mcp-server',
    version: '0.1.0',
  },
  {
    capabilities: {
      tools: {}, // This server provides tools
    },
  }
);

/**
 * Tool Registration: List Available Tools
 *
 * When GitHub Copilot connects, it asks: "What tools do you have? or What can you do?"
 * This handler responds with the list of available tools.
 * This tells Copilot what this tool can do and what information it needs.
 */
server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: [
      {
        name: 'generate_project_structure',
        description: 'Generate project files following established code patterns from real examples. Analyzes your request and creates code matching your existing codebase style.',
        inputSchema: {
          type: 'object',
          properties: {
            prompt: { // (required): What the user wants to generate
              type: 'string',
              description: 'Description of what to generate (e.g., "Create a payments service with CRUD operations")',
            },
            stack: { //(optional): Which technology (.NET, Python, JavaScript)
              type: 'string',
              description: 'Target framework/stack',
              enum: ['dotnet', 'python', 'javascript'],
              default: 'javascript',
            },
            targetPath: { //(optional): Where to put the generated files
              type: 'string',
              description: 'Optional: base path where files should be generated',
            },
          },
          required: ['prompt'],
        },
      },
    ],
  };
});

/**
 * Configuration
 */
const LANGGRAPH_API_URL = 'http://127.0.0.1:8000';

/**
 * Tool Execution: Handle Tool Calls
 *
 * When GitHub Copilot invokes a tool (say: "run this tool with these arguments"), this handler processes it.
 * Now connects to the LangGraph engine via HTTP to generate actual code.
 * Copilot may say: "Run the generate_project_structure tool with these arguments"
 */
server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  if (name === 'generate_project_structure') {
    try {
      const { prompt, stack = 'javascript', targetPath = 'src' } = args;

      // Call the LangGraph engine's /generate endpoint
      console.error(`Calling LangGraph API: ${LANGGRAPH_API_URL}/generate`);
      console.error(`Request: prompt="${prompt}", stack="${stack}", targetPath="${targetPath}"`);

      const response = await axios.post(`${LANGGRAPH_API_URL}/generate`, {
        prompt: prompt,
        stack: stack,
        target_path: targetPath,
      });

      const data = response.data;

      // Check if generation was successful
      if (!data.success) {
        return {
          content: [
            {
              type: 'text',
              text: `Generation failed:\n${data.errors?.join('\n') || 'Unknown error'}`,
            },
          ],
          isError: true,
        };
      }

      // Format the response for GitHub Copilot
      // Build a summary of generated files
      let resultText = `${data.summary}\n\n`;
      resultText += `Generated ${data.files.length} file(s):\n\n`;

      data.files.forEach((file, index) => {
        resultText += `${index + 1}. ${file.action.toUpperCase()}: ${file.path}\n`;
      });

      resultText += '\n--- File Contents ---\n\n';

      data.files.forEach((file) => {
        resultText += `\n=== ${file.path} ===\n`;
        resultText += `${file.content}\n`;
      });

      return {
        content: [
          {
            type: 'text',
            text: resultText,
          },
        ],
      };
    } catch (error) {
      // Handle connection errors and other exceptions
      let errorMessage = `Error: ${error.message}`;

      if (error.code === 'ECONNREFUSED') {
        errorMessage = `Could not connect to LangGraph engine at ${LANGGRAPH_API_URL}.\n\nMake sure the Python server is running:\n  cd langgraph-engine\n  .\\run.ps1`;
      } else if (error.response) {
        errorMessage = `LangGraph API error: ${error.response.status} - ${error.response.statusText}\n${JSON.stringify(error.response.data, null, 2)}`;
      }

      return {
        content: [
          {
            type: 'text',
            text: errorMessage,
          },
        ],
        isError: true,
      };
    }
  }

  // If tool name doesn't match, return error
  return {
    content: [
      {
        type: 'text',
        text: `Unknown tool: ${name}`,
      },
    ],
    isError: true,
  };
});

/**
 * Start the Server
 *
 * The server uses STDIO (standard input/output) transport (communication protocol) to communicate.
 * This means it reads from stdin and writes to stdout - the way
 * GitHub Copilot expects to communicate with MCP servers.
 */
async function main() {
  const transport = new StdioServerTransport(); //Creates a STDIO transport (communication channel)
  await server.connect(transport); //Connects the server to it

  // This message goes to stderr (for debugging), not stdout (protocol messages)
  // Logs a message to stderr (not stdout, because stdout is used for protocol messages)
  // stdout: Used for MCP protocol messages (Copilot reads this)
  // stderr: Used for debugging messages (we can read this, Copilot ignores it)
  console.error('Project Generator MCP Server running on stdio');
}

// Start the server
main().catch((error) => {
  console.error('Fatal error:', error);
  process.exit(1);
});

/*
Sample Workflow:
-------------------
GitHub Copilot starts MCP server
    ↓
main() function runs
    ↓
Server listens on STDIO
    ↓
Copilot asks: "What tools do you have?"
    ↓
ListToolsRequestSchema handler responds: "I have generate_project_structure"
    ↓
Copilot: "Run generate_project_structure with prompt='Create payments service'"
    ↓
CallToolRequestSchema handler runs
    ↓
HTTP POST to http://127.0.0.1:8000/generate
    ↓
LangGraph Engine (Python FastAPI) processes request
    ↓
Returns JSON with generated files
    ↓
MCP server formats response for Copilot
    ↓
Copilot displays the generated code to user

*/