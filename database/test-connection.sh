#!/bin/bash

# Test database connection and show seeded data

# Load environment variables
if [ -f .env ]; then
    set -a
    source .env
    set +a
else
    # Fallback values
    export SA_PASSWORD="StrongPassword123!"
fi

echo "🔍 Testing Transaction Explorer Database Connection..."
echo ""

# Check if container is running
if ! docker ps | grep -q "transaction-explorer-sqlserver-1\|transaction-explorer_sqlserver_1"; then
    echo "❌ Database container is not running."
    echo "   Run './start-database.sh' first."
    exit 1
fi

echo "📊 Testing database connection..."
docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT @@VERSION" -h-1 -C

echo ""
echo "📋 Transaction count:"
docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -d TransactionExplorer -Q "SELECT COUNT(*) AS TransactionCount FROM Transactions" -h-1 -C

echo ""
echo "💱 Exchange rate count:"
docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -d TransactionExplorer -Q "SELECT COUNT(*) AS ExchangeRateCount FROM ExchangeRates" -h-1 -C

echo ""
echo "📈 Sample transactions:"
docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -d TransactionExplorer -Q "SELECT TOP 5 Id, Description, TransactionDate, PurchaseAmount FROM Transactions ORDER BY TransactionDate DESC" -h-1 -C

echo ""
echo "🌍 Available currencies:"
docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -d TransactionExplorer -Q "SELECT DISTINCT CountryCurrencyDesc FROM ExchangeRates ORDER BY CountryCurrencyDesc" -h-1 -C

echo ""
echo "✅ Database connection test completed!"
