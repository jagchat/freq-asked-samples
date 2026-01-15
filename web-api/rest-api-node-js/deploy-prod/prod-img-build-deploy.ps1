if (-not $env:GITHUB_USERNAME) {
    Write-Host "ERROR: GITHUB_USERNAME environment variable is not set!" -ForegroundColor Red
    Write-Host "Please set it first: `$env:GITHUB_USERNAME = 'your-github-username'" -ForegroundColor Yellow
    exit 1
}

if (-not $env:GITHUB_TOKEN) {
    Write-Host "ERROR: GITHUB_TOKEN environment variable is not set!" -ForegroundColor Red
    Write-Host "Please set it first: `$env:GITHUB_TOKEN = 'your-github-token'" -ForegroundColor Yellow
    Write-Host "Then login: `$env:GITHUB_TOKEN | docker login ghcr.io -u `$env:GITHUB_USERNAME --password-stdin" -ForegroundColor Yellow
    exit 1
}

Write-Host "Removing existing local images..." -ForegroundColor Cyan
docker rmi ghcr.io/$env:GITHUB_USERNAME/demo-rest-api-node-js-prod:latest -f
docker rmi ghcr.io/$env:GITHUB_USERNAME/demo-rest-api-ops-db-prod:latest -f

Write-Host "Building API image..." -ForegroundColor Cyan
cd ..\src
docker build -t ghcr.io/$env:GITHUB_USERNAME/demo-rest-api-node-js-prod:latest .

Write-Host "Pushing API image..." -ForegroundColor Cyan
docker push ghcr.io/$env:GITHUB_USERNAME/demo-rest-api-node-js-prod:latest

Write-Host "Building DB image..." -ForegroundColor Cyan
cd ..\storage\ops-db
docker build -f Dockerfile -t ghcr.io/$env:GITHUB_USERNAME/demo-rest-api-ops-db-prod:latest .

Write-Host "Pushing DB image..." -ForegroundColor Cyan
docker push ghcr.io/$env:GITHUB_USERNAME/demo-rest-api-ops-db-prod:latest

Write-Host "Build and deploy completed successfully!" -ForegroundColor Green
cd ..\..\deploy-prod

