#!/bin/bash

# Troubleshooting script for SQL Server connection issues

echo "🔧 SQL Server Connection Troubleshooting"
echo "========================================"

# Load environment variables
if [ -f .env ]; then
    set -a
    source .env
    set +a
    echo "✅ Loaded environment variables from .env"
else
    export SA_PASSWORD="StrongPassword123!"
    echo "⚠️  Using fallback password"
fi

echo ""
echo "1. Container Status:"
echo "-------------------"
docker compose -p transaction-explorer ps

echo ""
echo "2. SQL Server Container Logs (last 20 lines):"
echo "----------------------------------------------"
docker compose -p transaction-explorer logs --tail=20 sqlserver

echo ""
echo "3. DB Init Container Logs:"
echo "-------------------------"
docker compose -p transaction-explorer logs db-init

echo ""
echo "4. Testing Connection from Host:"
echo "-------------------------------"
if command -v sqlcmd &> /dev/null; then
    sqlcmd -S localhost,1433 -U SA -P "$SA_PASSWORD" -Q "SELECT @@VERSION"
else
    echo "❌ sqlcmd not installed on host system"
fi

echo ""
echo "5. Testing Connection from Container:"
echo "------------------------------------"
if docker ps | grep -q "transaction-explorer-sqlserver-1"; then
    echo "Attempting connection..."
    docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT @@VERSION, @@SERVERNAME" -h-1 -C
    
    if [ $? -eq 0 ]; then
        echo "✅ Connection successful!"
        
        echo ""
        echo "6. Database Status:"
        echo "------------------"
        docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT name, database_id, create_date FROM sys.databases WHERE name IN ('master', 'TransactionExplorer')" -h-1 -C
    else
        echo "❌ Connection failed"
        echo ""
        echo "Common Solutions:"
        echo "- Wait longer for SQL Server to fully start (can take 60+ seconds)"
        echo "- Check if password contains special characters that need escaping"
        echo "- Restart the containers: docker compose -p transaction-explorer down && docker compose -p transaction-explorer up -d"
        echo "- Check Docker Desktop resources (memory should be 2GB+)"
    fi
else
    echo "❌ SQL Server container is not running"
    echo "Run: docker compose -p transaction-explorer up -d"
fi

echo ""
echo "7. Port Check:"
echo "-------------"
if command -v nc &> /dev/null; then
    if nc -z localhost 1433; then
        echo "✅ Port 1433 is reachable"
    else
        echo "❌ Port 1433 is not reachable"
    fi
else
    echo "ℹ️  Install 'nc' (netcat) to test port connectivity"
fi

echo ""
echo "8. Environment Variables:"
echo "------------------------"
echo "SA_PASSWORD: ${SA_PASSWORD:0:5}..." # Show only first 5 chars for security
echo "ACCEPT_EULA: $ACCEPT_EULA"
echo "MSSQL_PID: $MSSQL_PID"
