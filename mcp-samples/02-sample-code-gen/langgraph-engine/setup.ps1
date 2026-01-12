# Setup Python Virtual Environment and Install Dependencies
#
# This script:
# 1. Creates a Python virtual environment (.venv)
# 2. Activates it
# 3. Installs all dependencies from requirements.txt

Write-Host "Setting up LangGraph Engine Python Environment..." -ForegroundColor Green
Write-Host ""

# Check if Python is installed
Write-Host "Checking Python installation..." -ForegroundColor Cyan
$pythonCmd = Get-Command python -ErrorAction SilentlyContinue
if (-not $pythonCmd) {
    Write-Host "ERROR: Python is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Python 3.11+ from https://www.python.org/downloads/" -ForegroundColor Yellow
    exit 1
}

# Check Python version
$pythonVersion = python --version
Write-Host "Found: $pythonVersion" -ForegroundColor Green
Write-Host ""

# Create virtual environment if it doesn't exist
if (-Not (Test-Path ".venv")) {
    Write-Host "Creating virtual environment..." -ForegroundColor Cyan
    python -m venv .venv
    Write-Host "Virtual environment created at .venv/" -ForegroundColor Green
} else {
    Write-Host "Virtual environment already exists at .venv/" -ForegroundColor Yellow
}
Write-Host ""

# Activate virtual environment
Write-Host "Activating virtual environment..." -ForegroundColor Cyan
& .\.venv\Scripts\Activate.ps1

# # Upgrade pip
# Write-Host ""
# Write-Host "Upgrading pip..." -ForegroundColor Cyan
# python -m pip install --upgrade pip

# Install dependencies
Write-Host ""
Write-Host "Installing dependencies from requirements.txt..." -ForegroundColor Cyan
pip install -r requirements.txt

Write-Host ""
Write-Host "Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Copy .env.example to .env and fill in your API keys" -ForegroundColor White
Write-Host "   cp .env.example .env" -ForegroundColor Gray
Write-Host "2. Edit .env with your preferred LLM provider and API keys" -ForegroundColor White
Write-Host "3. Run the server with .\run.ps1" -ForegroundColor White
Write-Host ""
