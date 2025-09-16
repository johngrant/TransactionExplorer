#!/bin/bash

# Build and deploy script for Transaction Explorer Frontend

set -e

echo "🚀 Starting Transaction Explorer Frontend deployment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
IMAGE_NAME="transaction-explorer-frontend"
IMAGE_TAG="latest"
CONTAINER_NAME="transaction-explorer-frontend"

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

# Check if container is already running
if docker compose -p transaction-explorer ps | grep -q ${CONTAINER_NAME}; then
    print_warning "Container ${CONTAINER_NAME} is already running."
    print_status "Stopping existing container..."
    docker compose -p transaction-explorer stop
    docker compose -p transaction-explorer rm -f
fi

# Start the frontend service
print_status "Starting frontend container..."
if docker compose -p transaction-explorer up -d; then
    print_success "Frontend container started successfully ✓"
else
    print_error "Failed to start frontend container"
    exit 1
fi

# Wait a moment for the container to fully start
sleep 3

# Check if container is running
if docker compose -p transaction-explorer ps | grep -q "Up"; then
    print_success "Frontend is running at http://localhost:3000 ✓"

    # Show container status
    print_status "Container status:"
    docker compose -p transaction-explorer ps
else
    print_error "Frontend container failed to start properly"
    print_status "Checking logs..."
    docker compose -p transaction-explorer logs
    exit 1
fi

echo ""
print_success "Frontend deployment completed successfully! 🎉"
echo "Access the application at: http://localhost:3000"
