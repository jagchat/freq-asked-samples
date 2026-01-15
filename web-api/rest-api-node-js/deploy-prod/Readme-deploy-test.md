# How to start

### Test without deployment

NOTE: The expectation is that they are already deployed

```
cd deploy-prod
.\prod-img-test.start.ps1
```

### Deploying to docker registry

NOTE: Following steps are to use with Windows/Powershell

**Step 1: Create GitHub Personal Access Token**

- Go to: https://github.com/settings/tokens/new
- Token name: docker-registry-access
- Expiration: Your choice (90 days or custom)
- Select scopes: write:packages, read:packages
- Click Generate token and copy it

**Step 2: Login to GitHub Container Registry**

```
$env:GITHUB_TOKEN = "YOUR_TOKEN_HERE"
$env:GITHUB_USERNAME = "jagchat"
$env:GITHUB_TOKEN | docker login ghcr.io -u $env:GITHUB_USERNAME --password-stdin
```

**Step 2: Build and Deploy**

```
cd deploy-prod
..\prod-img-build-deploy.ps1
```

**Step 5: Make Images Public**

NOTE: This will be necessary only for the first time

- Go to https://github.com/jagchat?tab=packages
- Click on rest-api-node-js → Package settings → Change visibility → Public
- Click on demo-ops-db → Package settings → Change visibility → Public

That's it! Your images are now publicly available and anyone can pull them with:

```
docker pull ghcr.io/jagchat/rest-api-node-js:latest
docker pull ghcr.io/jagchat/demo-ops-db:latest
```
