#!/usr/bin/env node

/**
 * Simple test script for MCP server
 *
 * This simulates what GitHub Copilot does when calling the MCP server:
 * 1. List available tools
 * 2. Call the generate_project_structure tool
 *
 * Prerequisites:
 * - LangGraph server must be running (cd langgraph-engine && .\run.ps1)
 */

import { spawn } from 'child_process';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

console.log('MCP Server Test');
console.log('=' .repeat(60));
console.log('');

// Start the MCP server as a child process
const mcpServer = spawn('node', [join(__dirname, 'src', 'index.mjs')], {
  stdio: ['pipe', 'pipe', 'inherit'] // stdin, stdout, stderr
});

let responseBuffer = '';

mcpServer.stdout.on('data', (data) => {
  responseBuffer += data.toString();

  // Try to parse JSON responses
  const lines = responseBuffer.split('\n');
  responseBuffer = lines.pop() || ''; // Keep incomplete line in buffer

  lines.forEach((line) => {
    if (line.trim()) {
      try {
        const response = JSON.parse(line);
        console.log('Response:', JSON.stringify(response, null, 2));
      } catch (e) {
        console.log('Raw output:', line);
      }
    }
  });
});

mcpServer.on('error', (error) => {
  console.error('Failed to start MCP server:', error);
  process.exit(1);
});

// Give the server a moment to start
setTimeout(() => {
  console.log('Sending request to list tools...');
  console.log('');

  // Send a ListTools request
  const listToolsRequest = {
    jsonrpc: '2.0',
    id: 1,
    method: 'tools/list',
    params: {}
  };

  mcpServer.stdin.write(JSON.stringify(listToolsRequest) + '\n');

  // Wait a bit, then send a CallTool request
  setTimeout(() => {
    console.log('');
    console.log('Sending request to generate code (DEFAULT mode - will create files)...');
    console.log('');

    const callToolRequest = {
      jsonrpc: '2.0',
      id: 2,
      method: 'tools/call',
      params: {
        name: 'generate_project_structure',
        arguments: {
          prompt: 'Create a simple User service with CRUD operations',
          stack: 'dotnet',
          targetPath: 'test-output'  // Use test-output to avoid polluting src
        }
      }
    };

    mcpServer.stdin.write(JSON.stringify(callToolRequest) + '\n');

    // Wait for response
    setTimeout(() => {
      console.log('');
      console.log('=' .repeat(60));
      console.log('Test complete!');
      console.log('');
      console.log('Note: Files created in test-output/ directory');
      mcpServer.kill();
      process.exit(0);
    }, 5000);
  }, 2000);
}, 1000);
