# Transaction Explorer - Backend Deployment

This directory contains Docker deployment configuration for the Transaction Explorer backend API.

## 🚀 Quick Start

### Prerequisites

- Docker and Docker Compose
- .NET 9.0 SDK (for development)
- Existing SQL Server database (running in Docker)

### Deployment

1. **Navigate to the backend directory**:
   ```bash
   cd backend
   ```

2. **Build and deploy**:
   ```bash
   ./up.sh
   ```

3. **To stop the services**:
   ```bash
   ./down.sh
   ```

3. **Or manually**:
   ```bash
   # Build the backend image
   docker build -f Dockerfile -t transaction-explorer-backend:latest .

   # Start services
   docker compose up -d

   # Check status
   docker compose ps
   ```

4. **Access the services**:
   - Backend API: http://localhost:5070 (with hot reload)
   - Database: Your existing SQL Server instance

5. **To stop everything**:
   ```bash
   ./down.sh
   ```

## 📁 File Structure

```
backend/
├── Dockerfile              # Multi-stage Docker build
├── .dockerignore           # Docker ignore patterns
├── docker compose.yml      # Docker Compose with hot reload
├── up.sh                   # Automated startup script
├── down.sh                 # Automated shutdown script
├── DEPLOYMENT.md           # This documentation file
└── ...
```

## 🐳 Docker Configuration

### Dockerfile Features

- **Multi-stage build**: Separates build and runtime environments
- **Security**: Runs as non-root user
- **Optimization**: Uses minimal runtime image
- **Health checks**: Built-in health monitoring
- **.NET 9.0**: Latest .NET runtime

### Docker Compose Service

- **transaction-explorer-backend**: API service with hot reload
- **Connects to existing SQL Server**: Uses your current database setup

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ASPNETCORE_URLS` | Binding URLs | `http://+:8080` |
| `ConnectionStrings__DefaultConnection` | Database connection string | SQL Server connection |

### Database Configuration

- **Database**: `TransactionExplorer`
- **User**: `sa`
- **Password**: Update in docker compose.yml
- **Port**: `1433`

## 📊 Monitoring and Health Checks

### Health Endpoints

- **Health Check**: `GET /health`
- **API Root**: `GET /` (weather forecast sample)

### Docker Health Checks

```bash
# Check container health
docker compose -p transaction-explorer ps

# View logs
docker compose -p transaction-explorer logs -f transaction-explorer-backend
```

## 🔒 Security Considerations

### Deployment

1. **Change default passwords**:
   - Update SQL Server credentials in `docker compose.yml`

2. **Use proper TLS certificates** for production endpoints
3. **Use secrets management** (e.g., Azure Key Vault, AWS Secrets Manager)

### Container Security

- Runs as non-root user
- Minimal attack surface with alpine-based images
- No unnecessary packages in runtime image

## 🚀 Scaling

### Docker Compose
```bash
# Note: Scaling is limited due to volume mounts for hot reload
# For multiple instances, consider removing volume mounts
```

## 🐛 Troubleshooting

### Common Issues

1. **Port already in use**:
   ```bash
   # Check what's using the port
   lsof -i :5070

   # Stop conflicting services
   docker compose -p transaction-explorer down
   ```

2. **Database connection issues**:
   ```bash
   # Check your existing SQL Server database
   # Make sure it's running and accessible
   ```

3. **Image build failures**:
   ```bash
   # Clean build cache
   docker builder prune

   # Rebuild without cache
   docker build --no-cache -f backend/Dockerfile -t transaction-explorer-backend:latest .
   ```

4. **Hot reload not working**:
   ```bash
   # Make sure the volume mount is correct
   docker compose -p transaction-explorer logs transaction-explorer-backend

   # Restart container
   docker compose -p transaction-explorer restart transaction-explorer-backend
   ```

## 📈 Performance Tuning

### Database Optimization

- Connection pooling is handled by .NET
- Consider read replicas for high-traffic scenarios
- Monitor query performance with PostgreSQL logs

### API Optimization

- Enable response compression
- Implement caching strategies
- Use async/await patterns consistently

## 🔄 CI/CD Integration

### GitHub Actions Example

```yaml
name: Build and Deploy
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3

    - name: Build and Deploy
      run: |
        docker build -f backend/Dockerfile -t transaction-explorer-backend:${{ github.sha }} .
        # Push to registry and deploy
```

## 📝 Maintenance

### Regular Tasks

1. **Update base images** regularly for security patches
2. **Monitor resource usage** and adjust limits as needed
3. **Backup database** regularly
4. **Review logs** for errors and performance issues

### Database Maintenance

```sql
-- For SQL Server
-- Check database size
SELECT
    DB_NAME(database_id) AS DatabaseName,
    Name AS Logical_Name,
    Physical_Name,
    (size*8)/1024 SizeMB
FROM sys.master_files
WHERE DB_NAME(database_id) = 'TransactionExplorer';

-- Analyze query performance
-- Check your specific queries for performance issues
```
