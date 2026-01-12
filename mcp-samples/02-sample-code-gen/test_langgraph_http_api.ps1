# Integration Test Script
# Tests the connection between MCP Server and LangGraph Engine

Write-Host "Integration Test: MCP Server <-> LangGraph Engine" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Step 1: Check if LangGraph server is running
Write-Host "Step 1: Checking if LangGraph server is running..." -ForegroundColor Yellow

try {
    $healthCheck = Invoke-RestMethod -Uri "http://127.0.0.1:8000/health" -Method Get -ErrorAction Stop
    Write-Host "✓ LangGraph server is running" -ForegroundColor Green
    Write-Host "  Status: $($healthCheck.status)" -ForegroundColor Gray
}
catch {
    Write-Host "✗ LangGraph server is NOT running" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please start the LangGraph server first:" -ForegroundColor Yellow
    Write-Host "  cd langgraph-engine" -ForegroundColor White
    Write-Host "  .\run.ps1" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host ""

# Step 2: Test the /generate endpoint directly
Write-Host "Step 2: Testing /generate endpoint directly..." -ForegroundColor Yellow

$testRequest = @{
    prompt = "Create a simple User service with CRUD operations"
    stack = "dotnet"
    target_path = "src"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://127.0.0.1:8000/generate" `
        -Method Post `
        -Body $testRequest `
        -ContentType "application/json" `
        -ErrorAction Stop

    Write-Host "✓ Successfully called /generate endpoint" -ForegroundColor Green
    Write-Host "  Success: $($response.success)" -ForegroundColor Gray
    Write-Host "  Files generated: $($response.files.Count)" -ForegroundColor Gray
    Write-Host "  Summary: $($response.summary)" -ForegroundColor Gray
    Write-Host ""

    # Show errors if any
    if ($response.errors -and $response.errors.Count -gt 0) {
        Write-Host "Errors:" -ForegroundColor Red
        foreach ($error in $response.errors) {
            Write-Host "  - $error" -ForegroundColor Red
        }
        Write-Host ""
    }

    # Show generated files
    Write-Host "Generated Files:" -ForegroundColor Cyan
    foreach ($file in $response.files) {
        Write-Host "  - $($file.action): $($file.path)" -ForegroundColor White
    }
}
catch {
    Write-Host "✗ Failed to call /generate endpoint" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "Integration Test Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next Step: Test the MCP server" -ForegroundColor Yellow
Write-Host "The MCP server will make the same HTTP call when invoked by GitHub Copilot." -ForegroundColor Gray
Write-Host ""
