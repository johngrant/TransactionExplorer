#!/bin/bash

# Stop script for Transaction Explorer Backend

set -e

echo "🛑 Stopping Transaction Explorer Backend..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Stop and remove containers
print_status "Stopping backend containers..."
if docker compose -p transaction-explorer down; then
    print_success "Backend containers stopped successfully ✓"
else
    print_error "Failed to stop backend containers"
    exit 1
fi

# Check if containers are stopped
if ! docker compose -p transaction-explorer ps | grep -q "Up"; then
    print_success "All backend containers are stopped ✓"
    echo ""
    echo "🎉 Backend shutdown completed successfully!"
    echo ""
    echo "To start again: ./up.sh"
else
    print_warning "Some containers may still be running"
    docker compose -p transaction-explorer ps
fi
