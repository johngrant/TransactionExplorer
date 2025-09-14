#!/bin/bash

# Build and deploy script for Transaction Explorer Backend

set -e

echo "🚀 Starting Transaction Explorer Backend deployment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
IMAGE_NAME="transaction-explorer-backend"
IMAGE_TAG="latest"

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker and try again."
    exit 1
fi

print_success "Docker is running ✓"

# Build the Docker image
print_status "Building Docker image..."
if docker build -f Dockerfile -t ${IMAGE_NAME}:${IMAGE_TAG} .; then
    print_success "Docker image built successfully ✓"
else
    print_error "Failed to build Docker image"
    exit 1
fi

# Deploy with Docker Compose
print_status "Deploying with Docker Compose..."

# Stop existing containers
if docker compose -p transaction-explorer ps | grep -q "transaction-explorer"; then
    print_status "Stopping existing containers..."
    docker compose -p transaction-explorer down
fi

# Start services
print_status "Starting services..."
if docker compose -p transaction-explorer up -d; then
    print_success "Services started successfully ✓"
else
    print_error "Failed to start services"
    exit 1
fi

# Wait for services to be ready
print_status "Waiting for services to be ready..."
sleep 20

# Check if services are running
if docker compose -p transaction-explorer ps | grep -q "Up"; then
    print_success "All services are running ✓"
    echo ""
    echo "🎉 Deployment completed successfully!"
    echo ""
    echo "Services:"
    echo "  Backend API: http://localhost:5070"
    echo "  Database: Your existing SQL Server"
    echo ""
    echo "To view logs: docker compose -p transaction-explorer logs -f"
    echo "To stop: docker compose -p transaction-explorer down"
else
    print_error "Some services failed to start"
    docker compose -p transaction-explorer logs
    exit 1
fi
