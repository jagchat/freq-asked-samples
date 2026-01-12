# Run LangGraph Engine Server with Auto-Reload
# NOTE: run this script after "setup.ps1"

# This script:
# 1. Activates the virtual environment
# 2. Starts the FastAPI server with auto-reload enabled
# 3. Server automatically restarts when Python files change

Write-Host "Starting LangGraph Engine Server (Development Mode)..." -ForegroundColor Green
Write-Host "Server will auto-reload when you modify Python files" -ForegroundColor Cyan
Write-Host ""

# Check if virtual environment exists
if (-Not (Test-Path ".venv")) {
    Write-Host "ERROR: Virtual environment not found!" -ForegroundColor Red
    Write-Host "Please run .\setup.ps1 first to create the environment" -ForegroundColor Yellow
    exit 1
}

# Check if .env file exists
if (-Not (Test-Path ".env")) {
    Write-Host "WARNING: .env file not found!" -ForegroundColor Yellow
    Write-Host "Copy .env.example to .env and configure your API keys:" -ForegroundColor Yellow
    Write-Host "  cp .env.example .env" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Continuing anyway (server may fail without proper config)..." -ForegroundColor Yellow
    Write-Host ""
}

# Activate virtual environment
& .\.venv\Scripts\Activate.ps1

# Run the server with auto-reload
Write-Host "Starting FastAPI server on http://127.0.0.1:8000" -ForegroundColor Cyan
Write-Host "Auto-reload enabled - changes will restart the server" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host ""

# Run uvicorn with --reload flag for development
python -m uvicorn server:app --host 127.0.0.1 --port 8000 --reload
