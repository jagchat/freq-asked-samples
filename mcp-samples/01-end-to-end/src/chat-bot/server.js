import express from "express";
import { WebSocketServer } from "ws";
import { createServer } from "http";
import Anthropic from "@anthropic-ai/sdk";
import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StdioClientTransport } from "@modelcontextprotocol/sdk/client/stdio.js";
import dotenv from "dotenv";

dotenv.config();

const app = express();
const server = createServer(app);
const wss = new WebSocketServer({ server });

// Serve static files
app.use(express.static("public"));

// Start HTTP server first
const PORT = process.env.PORT || 3001;
server.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`);
});

const anthropic = new Anthropic({
  apiKey: process.env.ANTHROPIC_API_KEY,
});

// Store conversation history per connection
const conversations = new Map();

let mcpClient = null;
let mcpTools = [];

async function initMCP() {
  try {
    const command = process.env.MCP_SERVER_COMMAND || "node";
    const args = JSON.parse(process.env.MCP_SERVER_ARGS || "[]");

    const transport = new StdioClientTransport({
      command: command,
      args: args,
    });

    mcpClient = new Client(
      {
        name: "mcp-chatbot",
        version: "1.0.0",
      },
      {
        capabilities: {},
      }
    );

    await mcpClient.connect(transport);
    console.log("Connected to MCP server");

    // Get available tools
    const toolsResponse = await mcpClient.listTools();
    mcpTools = toolsResponse.tools.map((tool) => ({
      name: tool.name,
      description: tool.description,
      input_schema: tool.inputSchema,
    }));

    console.log(`Loaded ${mcpTools.length} tools from MCP server`);
  } catch (error) {
    console.error("Failed to initialize MCP client:", error);
    throw error;
  }
}

// Call MCP tool
async function callMCPTool(toolName, toolInput) {
  try {
    const result = await mcpClient.callTool({
      name: toolName,
      arguments: toolInput,
    });
    return result.content[0].text;
  } catch (error) {
    console.error("Error calling MCP tool:", error);
    throw error;
  }
}

// Handle chat with Claude
async function handleChat(message, conversationHistory) {
  const messages = [
    ...conversationHistory,
    {
      role: "user",
      content: message,
    },
  ];

  let response = await anthropic.messages.create({
    model: "claude-sonnet-4-20250514",
    max_tokens: 4096,
    tools: mcpTools,
    messages: messages,
  });

  // Handle tool calls
  while (response.stop_reason === "tool_use") {
    const toolUse = response.content.find((block) => block.type === "tool_use");

    console.log(`Claude is calling tool: ${toolUse.name}`);

    // Call MCP tool
    const toolResult = await callMCPTool(toolUse.name, toolUse.input);

    // Add assistant response and tool result to messages
    messages.push({
      role: "assistant",
      content: response.content,
    });

    messages.push({
      role: "user",
      content: [
        {
          type: "tool_result",
          tool_use_id: toolUse.id,
          content: toolResult,
        },
      ],
    });

    // Get next response from Claude
    response = await anthropic.messages.create({
      model: "claude-sonnet-4-20250514",
      max_tokens: 4096,
      tools: mcpTools,
      messages: messages,
    });
  }

  // Extract final text response
  const textContent = response.content.find((block) => block.type === "text");
  return {
    text: textContent.text,
    messages: messages,
  };
}

// WebSocket connection handler
wss.on("connection", (ws) => {
  const connectionId = Date.now().toString();
  conversations.set(connectionId, []);

  console.log("New client connected:", connectionId);

  ws.on("message", async (data) => {
    try {
      const { type, content } = JSON.parse(data);

      if (type === "message") {
        const conversationHistory = conversations.get(connectionId);

        // Get response from Claude
        const result = await handleChat(content, conversationHistory);

        // Update conversation history
        conversations.set(connectionId, [
          ...result.messages,
          {
            role: "assistant",
            content: result.text,
          },
        ]);

        // Send response back to client
        ws.send(
          JSON.stringify({
            type: "assistant",
            content: result.text,
          })
        );
      }
    } catch (error) {
      console.error("Error handling message:", error);
      ws.send(
        JSON.stringify({
          type: "error",
          content: error.message,
        })
      );
    }
  });

  ws.on("close", () => {
    conversations.delete(connectionId);
    console.log("Client disconnected:", connectionId);
  });
});

initMCP()
  .then(() => {
    console.log("MCP client initialized successfully");
  })
  .catch((error) => {
    console.error("Failed to start server:", error);
    process.exit(1);
  });
