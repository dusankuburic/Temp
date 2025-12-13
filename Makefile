# Makefile for Temp Project
# Cross-platform development helper

.PHONY: help install start stop restart logs rebuild clean db-migrate db-reset health test deploy-prod

# Default target
help:
	@echo "Temp Project - Development Commands"
	@echo ""
	@echo "Setup:"
	@echo "  make install       Install dependencies"
	@echo "  make env           Create .env from template"
	@echo ""
	@echo "Development:"
	@echo "  make start         Start development environment"
	@echo "  make stop          Stop development environment"
	@echo "  make restart       Restart development environment"
	@echo "  make logs          View all logs"
	@echo "  make logs-api      View API logs"
	@echo "  make logs-spa      View SPA logs"
	@echo ""
	@echo "Docker:"
	@echo "  make rebuild       Rebuild Docker containers"
	@echo "  make clean         Clean up Docker resources"
	@echo ""
	@echo "Database:"
	@echo "  make db-migrate    Run database migrations"
	@echo "  make db-reset      Reset database (WARNING: destroys data)"
	@echo ""
	@echo "Testing:"
	@echo "  make test          Run all tests"
	@echo "  make test-api      Run API tests"
	@echo "  make test-spa      Run SPA tests"
	@echo "  make health        Check service health"
	@echo ""
	@echo "Production:"
	@echo "  make deploy-prod   Deploy to production"
	@echo ""

# Check if .env exists
env:
	@if [ ! -f .env ]; then \
		echo "Creating .env from template..."; \
		cp .env.template .env; \
		echo ".env file created. Please update it with your values."; \
	else \
		echo ".env file already exists."; \
	fi

# Install dependencies
install:
	@echo "Installing .NET dependencies..."
	@dotnet restore
	@echo "Installing Angular dependencies..."
	@cd Temp-SPA && npm install
	@echo "Dependencies installed."

# Start development environment
start: env
	@echo "Starting development environment..."
	@docker compose up -d
	@echo "Waiting for services..."
	@sleep 10
	@docker compose ps
	@echo ""
	@echo "Development environment ready!"
	@echo "  - App UI (HTTP):  http://localhost:4323"
	@echo "  - App UI (HTTPS): https://localhost:4443"
	@echo "  - API (HTTPS):    https://localhost:5001/swagger"
	@echo "  - Redis UI:       http://localhost:8081"

# Stop development environment
stop:
	@echo "Stopping development environment..."
	@docker compose down
	@echo "Stopped."

# Restart services
restart: stop start

# View logs
logs:
	@docker compose logs -f

logs-api:
	@docker compose logs -f temp.api

logs-spa:
	@docker compose logs -f temp.spa.angular

# Rebuild containers
rebuild:
	@echo "Rebuilding containers..."
	@docker compose build --no-cache
	@echo "Rebuild complete. Run 'make start' to start services."

# Clean up
clean:
	@echo "Cleaning up Docker resources..."
	@docker compose down -v
	@docker system prune -f
	@echo "Cleanup complete."

# Database migrations
db-migrate:
	@echo "Running database migrations..."
	@docker exec temp.api dotnet ef database update
	@echo "Migrations applied."

# Reset database
db-reset:
	@echo "Resetting database..."
	@docker exec temp.api dotnet ef database drop --force
	@docker exec temp.api dotnet ef database update
	@echo "Database reset complete."

# Health check
health:
	@echo "Checking service health..."
	@docker compose ps
	@echo ""
	@curl -k -f https://localhost:5001/health || echo "API: Unhealthy"
	@curl -f http://localhost:4323/healthcheck || echo "SPA: Unhealthy"

# Run all tests
test: test-api test-spa

# Run API tests
test-api:
	@echo "Running .NET tests..."
	@dotnet test

# Run SPA tests
test-spa:
	@echo "Running Angular tests..."
	@cd Temp-SPA && npm run test -- --watch=false --browsers=ChromeHeadless

# Deploy to production
deploy-prod: env
	@echo "Deploying to production..."
	@docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
	@echo "Production deployment complete."
