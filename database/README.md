# Transaction Explorer Database

This directory contains the database setup for the Transaction Explorer application using Microsoft SQL Server.

## Architecture

The database consists of two main tables:

### Transactions
- **Id**: Primary key (auto-increment)
- **Description**: Transaction description (max 50 characters)
- **TransactionDate**: Date of the transaction
- **PurchaseAmount**: Amount in USD (positive decimal with 2 decimal places)
- **CreatedAt/UpdatedAt**: Audit timestamps

### ExchangeRates
- **Id**: Primary key (auto-increment) 
- **RecordDate**: Date the rate was recorded
- **CountryCurrencyDesc**: Country and currency description
- **ExchangeRate**: Exchange rate to USD
- **EffectiveDate**: Date the rate is effective
- **CreatedAt**: Audit timestamp

### Views
- **TransactionCurrencyView**: Joins transactions with exchange rates for currency conversion

## Getting Started

### Prerequisites
- Docker and Docker Compose installed
- Port 1433 available (SQL Server default port)

### Starting the Database

1. Navigate to the database directory:
   ```bash
   cd database
   ```

2. Start the SQL Server container:
   ```bash
   docker compose -p transaction-explorer up -d
   ```

3. Wait for initialization to complete (check logs):
   ```bash
   docker compose -p transaction-explorer logs -f db-init
   ```

### Connection Details

- **Server**: localhost,1433
- **Database**: TransactionExplorer
- **Username**: SA
- **Password**: StrongPassword123!

### Connection String Example
```
Server=localhost,1433;Database=TransactionExplorer;User Id=SA;Password=StrongPassword123!;TrustServerCertificate=true;
```

### Stopping the Database

```bash
docker compose -p transaction-explorer down
```

To remove all data:
```bash
docker compose -p transaction-explorer down -v
```

## Seed Data

The database is initialized with:
- **20 sample transaction records** with realistic business expenses
- **Exchange rate data** for major currencies (EUR, GBP, CAD, JPY, AUD)
- **Historical rates** covering the last 6 months to support currency conversion requirements

## Database Schema

### Currency Conversion Logic

The application supports converting transactions to different currencies using the Treasury Reporting Rates of Exchange API data format. The conversion logic:

1. Finds the most recent exchange rate ≤ transaction date
2. Rate must be within 6 months of transaction date
3. Converted amounts are rounded to 2 decimal places
4. Returns error if no suitable rate is found

### Indexes

- `IX_ExchangeRates_Currency_Date`: Optimizes currency conversion lookups
- `IX_ExchangeRates_EffectiveDate`: Optimizes date-based queries

## Maintenance

### Backing Up Data

```bash
# Export transaction data
docker exec transaction-explorer-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P 'StrongPassword123!' -d TransactionExplorer -Q "SELECT * FROM Transactions" -o /tmp/transactions.csv -h-1 -s"," -W -C

# Copy backup to host
docker cp transaction-explorer-sqlserver-1:/tmp/transactions.csv ./backup/
```

### Updating Exchange Rates

The exchange rates table can be updated with fresh data from the Treasury API. The application should periodically fetch and update this data to ensure accurate currency conversions.

### Health Checks

The SQL Server container includes health checks that verify the database is responding properly. Check container health with:

```bash
docker compose -p transaction-explorer ps
```
