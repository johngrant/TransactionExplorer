#!/bin/bash

# Shutdown script for Transaction Explorer Frontend

set -e

echo "🛑 Stopping Transaction Explorer Frontend..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
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

# Stop Docker Compose services
print_status "Stopping frontend services..."
if docker compose -p transaction-explorer down; then
    print_success "Frontend services stopped successfully ✓"
else
    print_warning "Docker Compose down completed with warnings"
fi

# Clean up any dangling images (optional)
if docker images --filter "dangling=true" -q | head -1 | grep -q .; then
    print_status "Cleaning up dangling images..."
    docker image prune -f
    print_success "Dangling images cleaned up ✓"
fi

echo ""
print_success "Frontend shutdown completed successfully! ✓"
