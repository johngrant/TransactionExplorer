#!/bin/bash

# Master startup script for Transaction Explorer
# Starts database first, then backend

set -e

echo "🚀 Starting Transaction Explorer System..."

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

# Start database first
print_status "Starting database..."
echo "----------------------------------------"
cd database
if ./up.sh; then
    print_success "Database started successfully ✓"
else
    print_error "Failed to start database"
    exit 1
fi

# Wait for database to be fully ready
print_status "Waiting for database to be fully ready..."
sleep 10

# Start backend
cd ../backend
print_status "Starting backend..."
echo "----------------------------------------"
if ./up.sh; then
    print_success "Backend started successfully ✓"
else
    print_error "Failed to start backend"
    exit 1
fi

# Return to root directory
cd ..

echo ""
echo "🎉 Transaction Explorer System is now running!"
echo ""
echo "Services:"
echo "  📊 Database:    localhost:1433"
echo "  🚀 Backend API: http://localhost:5070"
echo ""
echo "To stop the system: ./down.sh"
echo "To check status:   docker ps --filter name=transaction-explorer"
