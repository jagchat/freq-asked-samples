# Run MCP Server
#
# This script starts the MCP server normally (without auto-restart).
# For development with auto-restart on file changes, use run-watch.ps1 instead.
# Press Ctrl+C to stop the server.

Write-Host "Starting Project Generator MCP Server..." -ForegroundColor Green
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

# Run the server
node src/index.mjs
