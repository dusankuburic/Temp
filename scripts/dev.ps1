# Development helper script for Temp project (PowerShell)

param(
    [Parameter(Position=0)]
    [string]$Command,

    [Parameter(Position=1)]
    [string]$Service
)

# Colors
$GREEN = "Green"
$YELLOW = "Yellow"
$RED = "Red"

function Log-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor $GREEN
}

function Log-Warn {
    param([string]$Message)
    Write-Host "[WARN] $Message" -ForegroundColor $YELLOW
}

function Log-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor $RED
}

function Check-Env {
    if (-not (Test-Path .env)) {
        Log-Warn ".env file not found. Creating from template..."
        Copy-Item .env.template .env
        Log-Info "Created .env file. Please update it with your values."
    } else {
        Log-Info ".env file found."
    }
}

function Start-Dev {
    Log-Info "Starting development environment..."
    Check-Env
    docker compose up -d
    Log-Info "Waiting for services to be healthy..."
    Start-Sleep -Seconds 10
    docker compose ps
    Log-Info ""
    Log-Info "Development environment is ready!"
    Log-Info "  - App UI (HTTP):  http://localhost:4323"
    Log-Info "  - App UI (HTTPS): https://localhost:4443"
    Log-Info "  - API (HTTPS):    https://localhost:5001/swagger"
    Log-Info "  - Redis UI:       http://localhost:8081"
}

function Stop-Dev {
    Log-Info "Stopping development environment..."
    docker compose down
    Log-Info "Development environment stopped."
}

function Restart-Dev {
    Log-Info "Restarting development environment..."
    Stop-Dev
    Start-Dev
}

function Show-Logs {
    param([string]$ServiceName)

    if ([string]::IsNullOrEmpty($ServiceName)) {
        Log-Info "Showing logs for all services..."
        docker compose logs -f
    } else {
        Log-Info "Showing logs for $ServiceName..."
        docker compose logs -f $ServiceName
    }
}

function Rebuild-Containers {
    Log-Info "Rebuilding containers..."
    docker compose build --no-cache
    Log-Info "Rebuild complete. Run '.\scripts\dev.ps1 start' to start services."
}

function Clean-Docker {
    Log-Warn "This will remove all containers, volumes, and images for this project."
    $confirmation = Read-Host "Are you sure? (y/N)"
    if ($confirmation -eq 'y' -or $confirmation -eq 'Y') {
        Log-Info "Cleaning up Docker resources..."
        docker compose down -v
        docker system prune -f
        Log-Info "Cleanup complete."
    } else {
        Log-Info "Cleanup cancelled."
    }
}

function Run-DbMigrate {
    Log-Info "Running database migrations..."
    docker exec temp.api dotnet ef database update
    Log-Info "Migrations applied successfully."
}

function Reset-Database {
    Log-Warn "This will DROP the database and recreate it!"
    $confirmation = Read-Host "Are you sure? (y/N)"
    if ($confirmation -eq 'y' -or $confirmation -eq 'Y') {
        Log-Info "Resetting database..."
        docker exec temp.api dotnet ef database drop --force
        docker exec temp.api dotnet ef database update
        Log-Info "Database reset complete."
    } else {
        Log-Info "Database reset cancelled."
    }
}

function Check-Health {
    Log-Info "Checking service health..."
    docker compose ps
    Write-Host ""

    Log-Info "Testing API health..."
    try {
        $response = Invoke-WebRequest -Uri https://localhost:5001/health -SkipCertificateCheck -ErrorAction Stop
        Write-Host "API: Healthy" -ForegroundColor Green
    } catch {
        Log-Error "API health check failed"
    }

    Log-Info "Testing SPA health..."
    try {
        $response = Invoke-WebRequest -Uri http://localhost:4323/healthcheck -ErrorAction Stop
        Write-Host "SPA: Healthy" -ForegroundColor Green
    } catch {
        Log-Error "SPA health check failed"
    }
}

function Install-Dependencies {
    Log-Info "Installing .NET dependencies..."
    dotnet restore

    Log-Info "Installing Angular dependencies..."
    Push-Location Temp-SPA
    npm install
    Pop-Location

    Log-Info "Dependencies installed."
}

function Run-Tests {
    Log-Info "Running .NET tests..."
    dotnet test

    Log-Info "Running Angular tests..."
    Push-Location Temp-SPA
    npm run test -- --watch=false
    Pop-Location
}

function Deploy-Production {
    Log-Warn "Deploying to production..."
    Check-Env
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
    Log-Info "Production deployment complete."
}

function Show-Help {
    Write-Host "Temp Development Helper Script (PowerShell)" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\scripts\dev.ps1 [command] [service]"
    Write-Host ""
    Write-Host "Commands:" -ForegroundColor Cyan
    Write-Host "  start         Start development environment"
    Write-Host "  stop          Stop development environment"
    Write-Host "  restart       Restart development environment"
    Write-Host "  logs [svc]    View logs (optionally for specific service)"
    Write-Host "  rebuild       Rebuild Docker containers"
    Write-Host "  clean         Clean up Docker resources"
    Write-Host "  db-migrate    Run database migrations"
    Write-Host "  db-reset      Reset database (WARNING: destroys data)"
    Write-Host "  health        Check service health"
    Write-Host "  install       Install dependencies"
    Write-Host "  test          Run tests"
    Write-Host "  deploy-prod   Deploy to production"
    Write-Host "  help          Show this help message"
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Cyan
    Write-Host "  .\scripts\dev.ps1 start"
    Write-Host "  .\scripts\dev.ps1 logs temp.api"
    Write-Host "  .\scripts\dev.ps1 health"
}

# Main script execution
switch ($Command) {
    "start" { Start-Dev }
    "stop" { Stop-Dev }
    "restart" { Restart-Dev }
    "logs" { Show-Logs -ServiceName $Service }
    "rebuild" { Rebuild-Containers }
    "clean" { Clean-Docker }
    "db-migrate" { Run-DbMigrate }
    "db-reset" { Reset-Database }
    "health" { Check-Health }
    "install" { Install-Dependencies }
    "test" { Run-Tests }
    "deploy-prod" { Deploy-Production }
    "help" { Show-Help }
    default {
        if ([string]::IsNullOrEmpty($Command)) {
            Show-Help
        } else {
            Log-Error "Unknown command: $Command"
            Write-Host ""
            Show-Help
            exit 1
        }
    }
}
