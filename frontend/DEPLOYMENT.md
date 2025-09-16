# Transaction Explorer Frontend Docker Setup

This directory contains the containerized React frontend application for Transaction Explorer.

## Quick Start

### Start Frontend
```bash
./up.sh
```

### Stop Frontend
```bash
./down.sh
```

## Configuration

The frontend container is configured for development with:
- Hot reload enabled
- Source code mounted as volume
- Port 3000 exposed
- Environment variables for API connection

## Docker Setup

### Dockerfile Stages
- **base**: Node.js 20 Alpine base image
- **deps**: Dependencies installation
- **build**: Production build stage
- **dev**: Development stage with hot reload
- **production**: Nginx-based production stage

### Development Mode (Default)
- Uses Vite dev server with hot reload
- Source code is mounted for live updates
- Runs on port 3000

### Production Mode
- Builds optimized static assets
- Serves via Nginx
- Includes proper caching headers and SPA routing

## Environment Variables

- `NODE_ENV`: Development/production environment
- `VITE_API_BASE_URL`: Backend API URL (default: http://localhost:5070)

## Ports

- **3000**: Development server (Vite)
- **80**: Production server (Nginx)

## Health Check

Production container includes health check at `/health` endpoint.

## Integration

The frontend is integrated into the main system:
- Started by root-level `up.sh` script
- Stopped by root-level `down.sh` script
- Depends on backend service when using docker-compose

## Troubleshooting

### Check logs
```bash
docker logs transaction-explorer-frontend
```

### Verify container status
```bash
docker ps --filter name=transaction-explorer-frontend
```

### Rebuild container
```bash
./down.sh
docker system prune -f
./up.sh
```
