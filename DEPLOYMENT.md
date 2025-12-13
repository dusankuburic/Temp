# Deployment Guide

## Table of Contents
- [Local Development](#local-development)
- [Production Deployment](#production-deployment)
- [SSL Certificate Setup](#ssl-certificate-setup)
- [Health Checks](#health-checks)
- [Troubleshooting](#troubleshooting)

---

## Local Development

### Quick Start
```bash
# 1. Copy environment template
cp .env.template .env

# 2. Start all services
docker compose up -d

# 3. Check service health
docker compose ps
```

### Endpoints
- **App UI (HTTP):** http://localhost:4323
- **App UI (HTTPS):** https://localhost:4443
- **API (HTTPS):** https://localhost:5001/swagger/index.html
- **Redis UI:** http://localhost:8081

### Stopping Services
```bash
docker compose down
```

### Viewing Logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f temp.api
```

---

## Production Deployment

### Step 1: Environment Configuration

```bash
# Copy environment template
cp .env.template .env

# Edit with production values
nano .env  # or vim .env
```

**Required production changes in `.env`:**
- `JWT_SECRET` - Generate new: `openssl rand -base64 64`
- `SQL_SA_PASSWORD` - Strong unique password
- `REDIS_PASSWORD` - Strong unique password
- `JWT_ISSUER` - Your production domain
- `SQL_CONNECTION_DOCKER` - Update with production password
- `AZURE_STORAGE_CONNECTION` - Real Azure Storage (not Azurite)

### Step 2: Deploy

```bash
# Deploy with production configuration
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Verify all services are healthy
docker compose ps
```

### Step 3: Verify Health Checks

All services should show `(healthy)` status:
```bash
docker compose ps
```

Expected output:
```
NAME                  STATUS
tempdb-prod           Up (healthy)
backend-azurite-prod  Up (healthy)
redis-cache-prod      Up (healthy)
temp.api-prod         Up (healthy)
temp.spa.angular-prod Up (healthy)
```

### Step 4: Test Endpoints

```bash
# Test Angular app health
curl http://localhost/health

# Test API health (development)
curl -k https://localhost:5001/health

# Production (on server)
curl -k https://localhost:5001/health
```

---

## SSL Certificate Setup

### Development (Self-Signed)

Self-signed certificates are automatically generated during Docker build.

**Angular SPA:** Generated in Dockerfile
**API:** Uses existing pfx certificate in `Temp.API/certs/`

### Production (Valid Certificates)

#### Option 1: Let's Encrypt (Free)

```bash
# Install certbot
sudo apt-get update
sudo apt-get install certbot

# Generate certificate
sudo certbot certonly --standalone -d yourdomain.com

# Certificates will be in /etc/letsencrypt/live/yourdomain.com/
```

**For Angular SPA (nginx):**

1. Update `docker-compose.prod.yml`:
```yaml
temp.spa.angular:
  volumes:
    - /etc/letsencrypt/live/yourdomain.com:/etc/nginx/ssl:ro
```

2. Update `Temp-SPA/nginx.conf`:
```nginx
ssl_certificate /etc/nginx/ssl/fullchain.pem;
ssl_certificate_key /etc/nginx/ssl/privkey.pem;
```

**For .NET API:**

Convert Let's Encrypt cert to pfx:
```bash
openssl pkcs12 -export \
  -out aspnetapp.pfx \
  -inkey /etc/letsencrypt/live/yourdomain.com/privkey.pem \
  -in /etc/letsencrypt/live/yourdomain.com/fullchain.pem \
  -password pass:YourCertPassword
```

#### Option 2: Commercial Certificate

1. Purchase certificate from CA (DigiCert, Comodo, etc.)
2. Follow CA instructions for generation
3. Mount certificates into containers as shown above

---

## Health Checks

### Service Health Check Endpoints

| Service | Health Check Command | Expected Result |
|---------|---------------------|-----------------|
| SQL Server | `sqlcmd -Q "SELECT 1"` | Returns 1 |
| Redis | `redis-cli ping` | PONG |
| Azurite | `nc -z localhost 10000` | Connection success |
| API | `curl https://localhost:8081/health` | HTTP 200 |
| Angular | `curl http://localhost/health` | "Healthy" |

### Manual Health Check

```bash
# Check all services
docker compose ps

# Check specific service health
docker inspect temp.api-prod --format='{{.State.Health.Status}}'

# View health check logs
docker inspect temp.api-prod --format='{{json .State.Health}}' | jq
```

### Debugging Unhealthy Services

```bash
# View service logs
docker compose logs temp.api

# Execute health check manually inside container
docker exec temp.api curl -k https://localhost:8081/health

# Enter container shell
docker exec -it temp.api /bin/bash
```

---

## Troubleshooting

### Services Not Starting

**Check logs:**
```bash
docker compose logs -f
```

**Common issues:**
- Port conflicts: Another service using ports 80, 443, 5000, 5001, 6379, 1433
- Missing .env file: Copy from .env.template
- Docker daemon not running: `systemctl status docker`

### Database Connection Issues

```bash
# Check if SQL Server is healthy
docker exec tempdb-prod /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourPassword' -Q 'SELECT 1' -C

# Check connection string in API
docker exec temp.api-prod printenv | grep ConnectionStrings
```

### Redis Connection Issues

```bash
# Test Redis connection
docker exec redis-cache-prod redis-cli -a YourRedisPassword ping

# Check Redis logs
docker compose logs redis
```

### SSL Certificate Issues

**"Certificate not trusted" in browser:**
- Expected for self-signed certificates
- Click "Advanced" → "Proceed anyway" for development
- For production, use valid certificates

**Certificate file not found:**
```bash
# Check if certificate exists in container
docker exec temp.spa.angular ls -la /etc/nginx/ssl/
docker exec temp.api ls -la /app/certs/
```

### Performance Issues

**Check resource usage:**
```bash
docker stats

# Increase container resources in Docker Desktop settings
# or modify docker-compose service definitions
```

### Networking Issues

**Check network connectivity:**
```bash
# List networks
docker network ls

# Inspect network
docker network inspect temp-network

# Test connectivity between containers
docker exec temp.api ping tempdb-prod
```

---

## Maintenance

### Backup

**SQL Server:**
```bash
docker exec tempdb-prod /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourPassword' \
  -Q "BACKUP DATABASE [Temp] TO DISK = '/var/opt/mssql/backup/temp.bak'" -C
```

**Redis:**
```bash
docker exec redis-cache-prod redis-cli -a YourRedisPassword BGSAVE
```

### Updates

```bash
# Pull latest images
docker compose pull

# Recreate containers with new images
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Remove old images
docker image prune -a
```

### Monitoring

**Enable monitoring with Prometheus/Grafana or Application Insights**

Example prometheus metrics endpoint for .NET:
```bash
# Add to Temp.API
dotnet add package prometheus-net.AspNetCore
```

---

## Support

For issues or questions:
1. Check logs: `docker compose logs`
2. Review this guide
3. Check application README.md
4. Review docker-compose configuration files
