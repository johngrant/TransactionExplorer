#!/bin/bash

# Master shutdown script for Transaction Explorer
# Stops backend first, then database

set -e

echo "🛑 Stopping Transaction Explorer System..."

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

# Stop backend first
print_status "Stopping backend..."
echo "----------------------------------------"
cd backend
if ./down.sh; then
    print_success "Backend stopped successfully ✓"
else
    print_warning "Backend may have already been stopped or failed to stop"
fi

# Stop database
cd ../database
print_status "Stopping database..."
echo "----------------------------------------"
if ./down.sh; then
    print_success "Database stopped successfully ✓"
else
    print_warning "Database may have already been stopped or failed to stop"
fi

# Return to root directory
cd ..

# Final cleanup - remove any orphaned containers from the project
print_status "Cleaning up any orphaned containers..."
if docker compose -p transaction-explorer down --remove-orphans > /dev/null 2>&1; then
    print_success "Cleanup completed ✓"
else
    print_warning "Cleanup may not have been necessary"
fi

echo ""
echo "🎉 Transaction Explorer System has been stopped!"
echo ""
echo "To start the system again: ./up.sh"
