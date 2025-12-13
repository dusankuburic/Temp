#!/bin/bash
# Development helper script for Temp project

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Helper functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if .env exists
check_env() {
    if [ ! -f .env ]; then
        log_warn ".env file not found. Creating from template..."
        cp .env.template .env
        log_info "Created .env file. Please update it with your values."
    else
        log_info ".env file found."
    fi
}

# Start development environment
start() {
    log_info "Starting development environment..."
    check_env
    docker compose up -d
    log_info "Waiting for services to be healthy..."
    sleep 10
    docker compose ps
    log_info ""
    log_info "Development environment is ready!"
    log_info "  - App UI (HTTP):  http://localhost:4323"
    log_info "  - App UI (HTTPS): https://localhost:4443"
    log_info "  - API (HTTPS):    https://localhost:5001/swagger"
    log_info "  - Redis UI:       http://localhost:8081"
}

# Stop development environment
stop() {
    log_info "Stopping development environment..."
    docker compose down
    log_info "Development environment stopped."
}

# Restart services
restart() {
    log_info "Restarting development environment..."
    stop
    start
}

# View logs
logs() {
    if [ -z "$1" ]; then
        log_info "Showing logs for all services..."
        docker compose logs -f
    else
        log_info "Showing logs for $1..."
        docker compose logs -f "$1"
    fi
}

# Rebuild containers
rebuild() {
    log_info "Rebuilding containers..."
    docker compose build --no-cache
    log_info "Rebuild complete. Run './scripts/dev.sh start' to start services."
}

# Clean up Docker resources
clean() {
    log_warn "This will remove all containers, volumes, and images for this project."
    read -p "Are you sure? (y/N) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        log_info "Cleaning up Docker resources..."
        docker compose down -v
        docker system prune -f
        log_info "Cleanup complete."
    else
        log_info "Cleanup cancelled."
    fi
}

# Database migrations
db_migrate() {
    log_info "Running database migrations..."
    docker exec temp.api dotnet ef database update
    log_info "Migrations applied successfully."
}

# Database reset
db_reset() {
    log_warn "This will DROP the database and recreate it!"
    read -p "Are you sure? (y/N) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        log_info "Resetting database..."
        docker exec temp.api dotnet ef database drop --force
        docker exec temp.api dotnet ef database update
        log_info "Database reset complete."
    else
        log_info "Database reset cancelled."
    fi
}

# Health check
health() {
    log_info "Checking service health..."
    docker compose ps
    echo ""

    log_info "Testing API health..."
    curl -k -f https://localhost:5001/health && echo "" || log_error "API health check failed"

    log_info "Testing SPA health..."
    curl -f http://localhost:4323/healthcheck && echo "" || log_error "SPA health check failed"
}

# Install dependencies
install() {
    log_info "Installing .NET dependencies..."
    dotnet restore

    log_info "Installing Angular dependencies..."
    cd Temp-SPA
    npm install
    cd ..

    log_info "Dependencies installed."
}

# Run tests
test() {
    log_info "Running .NET tests..."
    dotnet test

    log_info "Running Angular tests..."
    cd Temp-SPA
    npm run test -- --watch=false
    cd ..
}

# Production deployment
deploy_prod() {
    log_warn "Deploying to production..."
    check_env
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
    log_info "Production deployment complete."
}

# Show help
help() {
    echo "Temp Development Helper Script"
    echo ""
    echo "Usage: ./scripts/dev.sh [command]"
    echo ""
    echo "Commands:"
    echo "  start         Start development environment"
    echo "  stop          Stop development environment"
    echo "  restart       Restart development environment"
    echo "  logs [svc]    View logs (optionally for specific service)"
    echo "  rebuild       Rebuild Docker containers"
    echo "  clean         Clean up Docker resources"
    echo "  db-migrate    Run database migrations"
    echo "  db-reset      Reset database (WARNING: destroys data)"
    echo "  health        Check service health"
    echo "  install       Install dependencies"
    echo "  test          Run tests"
    echo "  deploy-prod   Deploy to production"
    echo "  help          Show this help message"
    echo ""
    echo "Examples:"
    echo "  ./scripts/dev.sh start"
    echo "  ./scripts/dev.sh logs temp.api"
    echo "  ./scripts/dev.sh health"
}

# Main script
case "$1" in
    start)
        start
        ;;
    stop)
        stop
        ;;
    restart)
        restart
        ;;
    logs)
        logs "$2"
        ;;
    rebuild)
        rebuild
        ;;
    clean)
        clean
        ;;
    db-migrate)
        db_migrate
        ;;
    db-reset)
        db_reset
        ;;
    health)
        health
        ;;
    install)
        install
        ;;
    test)
        test
        ;;
    deploy-prod)
        deploy_prod
        ;;
    help|--help|-h)
        help
        ;;
    *)
        log_error "Unknown command: $1"
        echo ""
        help
        exit 1
        ;;
esac
