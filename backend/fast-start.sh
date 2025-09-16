#!/bin/bash

# Fast start script for Transaction Explorer Backend (no Docker)
# This runs the API directly with dotnet run for fastest startup

set -e

echo "🚀 Fast-starting Transaction Explorer Backend..."

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET SDK and try again."
    exit 1
fi

print_success ".NET SDK found ✓"

# Navigate to WebApi directory
cd WebApi

# Set environment variables
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://localhost:5070

# Note: You'll need to update the connection string to point to your actual database
# export ConnectionStrings__DefaultConnection="Your-Database-Connection-String"

print_status "Starting API directly with dotnet run..."
print_status "API will be available at: http://localhost:5070"
print_status "Press Ctrl+C to stop"

# Run the application
dotnet run --project WebApi.csproj --urls http://localhost:5070
