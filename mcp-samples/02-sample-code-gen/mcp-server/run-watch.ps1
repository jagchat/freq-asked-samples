# Run MCP Server for Local Testing
#
# This script starts the MCP server in development mode with auto-restart on file changes.
# Press Ctrl+C to stop the server.

Write-Host "Starting Project Generator MCP Server..." -ForegroundColor Green
Write-Host "Server will auto-restart when you modify files" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

# Run the server with --watch flag for auto-restart
node --watch src/index.mjs
