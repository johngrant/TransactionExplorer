#!/bin/bash

# Stop script for Transaction Explorer Database

set -e

echo "🛑 Stopping Transaction Explorer Database..."

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

# Navigate to database directory
cd "$(dirname "$0")"

# Stop and remove containers
print_status "Stopping database containers..."
print_status "Stopping db and db-init containers specifically..."

# Stop the specific containers we want
if docker compose -p transaction-explorer stop sqlserver db-init; then
    print_success "Database and db-init containers stopped successfully ✓"
else
    print_error "Failed to stop database containers"
    exit 1
fi

# Remove the stopped containers
print_status "Removing stopped containers..."
if docker compose -p transaction-explorer rm -f sqlserver db-init; then
    print_success "Database containers removed successfully ✓"
else
    print_warning "Some containers may not have been removed properly"
fi

# Check if containers are stopped
if ! docker compose -p transaction-explorer ps sqlserver db-init | grep -q "Up"; then
    print_success "All database containers are stopped ✓"
    echo ""
    echo "🎉 Database shutdown completed successfully!"
    echo ""
    echo "To start again: ./up.sh"
else
    print_warning "Some containers may still be running"
    docker compose -p transaction-explorer ps sqlserver db-init
fi
