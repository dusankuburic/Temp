# CI/CD Documentation

## Overview

This project uses GitHub Actions for continuous integration and deployment. The workflows automatically build, test, and deploy the application.

## Workflows

### 1. CI/CD Pipeline (`ci.yml`)

**Triggers:**
- Push to `master` or `develop` branches
- Pull requests to `master` or `develop` branches
- Manual workflow dispatch

**Jobs:**

#### Build .NET API
- Restores dependencies
- Builds the project in Release mode
- Runs unit tests
- Publishes artifacts

#### Build Angular SPA
- Installs Node.js dependencies
- Runs linting
- Builds for production
- Runs unit tests
- Uploads build artifacts

#### Build Docker Images
- Only runs on push to `master` or `develop`
- Builds and pushes Docker images to GitHub Container Registry
- Uses layer caching for faster builds
- Tags images with branch name and commit SHA

#### Security Scanning
- Runs Trivy vulnerability scanner
- Uploads results to GitHub Security tab

#### Code Quality Analysis
- Checks .NET code formatting with `dotnet format`
- Runs ESLint on Angular code

### 2. Deployment Workflow (`deploy.yml`)

**Triggers:**
- New GitHub release published
- Manual workflow dispatch (choose staging or production)

**Jobs:**

#### Deploy Application
- Connects to deployment server via SSH
- Pulls latest code
- Deploys using docker-compose
- Runs database migrations
- Verifies deployment health
- Rolls back on failure

## Setup Instructions

### 1. GitHub Container Registry

Enable GitHub Packages for your repository:

1. Go to repository Settings → Actions → General
2. Under "Workflow permissions", select "Read and write permissions"
3. Save changes

### 2. Required Secrets

Add these secrets in GitHub repository Settings → Secrets and variables → Actions:

#### For Deployment

| Secret Name | Description | Example |
|------------|-------------|---------|
| `STAGING_HOST` | Staging server hostname/IP | `staging.example.com` |
| `PRODUCTION_HOST` | Production server hostname/IP | `prod.example.com` |
| `DEPLOY_USER` | SSH username for deployment | `deploy` |
| `DEPLOY_SSH_KEY` | Private SSH key for deployment | `-----BEGIN RSA PRIVATE KEY-----...` |
| `SLACK_WEBHOOK` | Slack webhook for notifications (optional) | `https://hooks.slack.com/...` |

#### For Production Environment Variables

| Secret Name | Description |
|------------|-------------|
| `JWT_SECRET` | Production JWT secret |
| `SQL_SA_PASSWORD` | Production SQL Server password |
| `REDIS_PASSWORD` | Production Redis password |
| `AZURE_STORAGE_CONNECTION` | Production Azure Storage connection |

### 3. SSH Key Setup

Generate SSH key for deployment:

```bash
# On your local machine
ssh-keygen -t rsa -b 4096 -C "deploy@temp-app" -f temp-deploy-key

# Copy public key to deployment server
ssh-copy-id -i temp-deploy-key.pub deploy@your-server.com

# Add private key to GitHub Secrets (entire contents of temp-deploy-key)
cat temp-deploy-key
```

### 4. Server Setup

On your deployment server:

```bash
# Create deployment directory
sudo mkdir -p /opt/temp-app
sudo chown deploy:deploy /opt/temp-app

# Clone repository
cd /opt/temp-app
git clone https://github.com/your-username/your-repo.git .

# Create .env file
cp .env.template .env
# Edit .env with production values
nano .env

# Install Docker and Docker Compose
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh
sudo usermod -aG docker deploy

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Logout and login to apply docker group
```

## Development Workflow

### Feature Development

```bash
# 1. Create feature branch
git checkout -b feature/my-feature

# 2. Make changes and commit
git add .
git commit -m "feat: add new feature"

# 3. Push to remote
git push origin feature/my-feature

# 4. Create pull request
# CI will automatically run tests and builds
```

### Code Quality Checks

Before committing, run local checks:

```bash
# Check .NET formatting
dotnet format --verify-no-changes

# Fix formatting issues
dotnet format

# Run Angular linting
cd Temp-SPA
npm run lint

# Fix linting issues
npm run lint -- --fix
```

### Running Tests Locally

```bash
# .NET tests
dotnet test

# Angular tests
cd Temp-SPA
npm run test -- --watch=false --browsers=ChromeHeadless

# Or use the helper scripts
./scripts/dev.sh test        # Linux/Mac
.\scripts\dev.ps1 test        # Windows
make test                     # Any platform with make
```

## Deployment Process

### Staging Deployment

1. Merge feature branch to `develop`
2. Docker images are automatically built and pushed
3. Manually trigger deployment workflow:
   - Go to Actions → Deploy to Production
   - Click "Run workflow"
   - Select "staging" environment
   - Click "Run workflow"

### Production Deployment

#### Option 1: Release (Recommended)

1. Merge `develop` to `master`
2. Create a new release on GitHub:
   - Go to Releases → Create a new release
   - Tag version (e.g., `v1.0.0`)
   - Generate release notes
   - Publish release
3. Deployment workflow automatically triggers

#### Option 2: Manual Deployment

1. Go to Actions → Deploy to Production
2. Click "Run workflow"
3. Select "production" environment
4. Click "Run workflow"

## Monitoring Deployments

### View Deployment Status

- Go to Actions tab in GitHub
- Click on the workflow run
- Monitor job progress in real-time

### Rollback

If deployment fails, the workflow automatically rolls back to the previous version. To manually rollback:

```bash
# SSH into server
ssh deploy@your-server.com

cd /opt/temp-app

# View git log
git log --oneline -10

# Rollback to previous commit
git reset --hard <previous-commit-sha>

# Redeploy
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Verify Deployment

After deployment:

```bash
# Check service health
docker compose ps

# Test API
curl -k https://your-domain.com:5001/healthcheck

# Test SPA
curl https://your-domain.com/healthcheck

# View logs
docker compose logs -f
```

## Troubleshooting

### Build Failures

**Symptom:** CI fails during build step

**Solutions:**
1. Check build logs in GitHub Actions
2. Run build locally: `dotnet build` or `npm run build`
3. Ensure all dependencies are up to date
4. Check for breaking changes in packages

### Test Failures

**Symptom:** Tests fail in CI but pass locally

**Solutions:**
1. Ensure test environment variables are set
2. Check for time zone or locale issues
3. Run tests in headless mode locally
4. Review test logs in GitHub Actions

### Deployment Failures

**Symptom:** Deployment workflow fails

**Solutions:**
1. Verify SSH connection: `ssh deploy@your-server.com`
2. Check server disk space: `df -h`
3. Verify Docker is running: `docker ps`
4. Check deployment logs
5. Ensure .env file exists on server

### Image Push Failures

**Symptom:** Cannot push Docker images to registry

**Solutions:**
1. Verify GitHub Packages is enabled
2. Check workflow permissions (read and write)
3. Ensure GITHUB_TOKEN has package write permission
4. Clear GitHub Actions cache

## Best Practices

### Commit Messages

Use conventional commits:

```
feat: add new feature
fix: resolve bug
docs: update documentation
style: format code
refactor: restructure code
test: add tests
chore: update dependencies
```

### Branch Strategy

- `master` - Production-ready code
- `develop` - Integration branch
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `hotfix/*` - Urgent production fixes

### Pull Request Checklist

- [ ] Code builds successfully
- [ ] All tests pass
- [ ] Code is properly formatted
- [ ] No linting errors
- [ ] Documentation updated
- [ ] Changes reviewed by peer

### Security

- Never commit secrets or .env files
- Rotate secrets regularly
- Use GitHub Secrets for sensitive data
- Enable branch protection rules
- Require status checks to pass before merging
- Enable security scanning

## Helper Scripts

### Linux/Mac

```bash
./scripts/dev.sh start     # Start development
./scripts/dev.sh stop      # Stop development
./scripts/dev.sh test      # Run tests
./scripts/dev.sh health    # Check health
```

### Windows PowerShell

```powershell
.\scripts\dev.ps1 start    # Start development
.\scripts\dev.ps1 stop     # Stop development
.\scripts\dev.ps1 test     # Run tests
.\scripts\dev.ps1 health   # Check health
```

### Make (Cross-platform)

```bash
make start                 # Start development
make stop                  # Stop development
make test                  # Run tests
make health                # Check health
```

## References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET CLI Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [Angular CLI Documentation](https://angular.io/cli)
