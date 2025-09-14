#!/bin/bash

# Transaction Explorer Database Setup Script

echo "🚀 Starting Transaction Explorer Database..."
echo "This will create a SQL Server container with sample data."
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running or paused. Please start/unpause Docker Desktop first."
    echo "   You can do this through the Docker Desktop app or the whale menu in your system tray."
    exit 1
fi

# Navigate to database directory
cd "$(dirname "$0")"

echo "📦 Building and starting SQL Server container..."
docker compose -p transaction-explorer up -d

echo "⏳ Waiting for SQL Server to start..."
sleep 10

echo "📊 Checking container status..."
docker compose -p transaction-explorer ps

echo ""
echo "🎉 Database setup complete!"
echo ""
echo "Connection Details:"
echo "  Server: localhost,1433"
echo "  Database: TransactionExplorer"
echo "  Username: SA"
echo "  Password: StrongPassword123!"
echo ""
echo "📋 To view initialization logs:"
echo "  docker compose -p transaction-explorer logs -f db-init"
echo ""
echo "🛑 To stop the database:"
echo "  docker compose -p transaction-explorer down"
echo ""
echo "🗑️  To remove all data:"
echo "  docker compose -p transaction-explorer down -v"
