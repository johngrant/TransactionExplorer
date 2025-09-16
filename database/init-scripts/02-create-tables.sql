-- Create tables for transaction explorer application

-- Transactions table based on requirements
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
BEGIN
    CREATE TABLE Transactions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomId NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(50) NOT NULL,
        TransactionDate DATE NOT NULL,
        PurchaseAmount DECIMAL(19,2) NOT NULL CHECK (PurchaseAmount > 0),
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
    PRINT 'Transactions table created successfully.';
END
ELSE
BEGIN
    PRINT 'Transactions table already exists.';
END
GO

-- Exchange rates table to support currency conversion
-- This will store Treasury Reporting Rates of Exchange data
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExchangeRates' AND xtype='U')
BEGIN
    CREATE TABLE ExchangeRates (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        RecordDate DATE NOT NULL,
        CountryCurrencyDesc NVARCHAR(100) NOT NULL,
        ExchangeRate DECIMAL(19,6) NOT NULL,
        EffectiveDate DATE NOT NULL,
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),

        -- Index for efficient lookups by currency and date
        INDEX IX_ExchangeRates_Currency_Date NONCLUSTERED (CountryCurrencyDesc, EffectiveDate DESC),
        INDEX IX_ExchangeRates_EffectiveDate NONCLUSTERED (EffectiveDate DESC)
    );
    PRINT 'ExchangeRates table created successfully.';
END
ELSE
BEGIN
    PRINT 'ExchangeRates table already exists.';
END
GO

-- Create a view for transaction currency conversion lookup
IF NOT EXISTS (SELECT * FROM sys.views WHERE name='TransactionCurrencyView')
BEGIN
    EXEC('
    CREATE VIEW TransactionCurrencyView AS
    SELECT
        t.Id,
        t.Description,
        t.TransactionDate,
        t.PurchaseAmount,
        t.CreatedAt,
        t.UpdatedAt,
        er.CountryCurrencyDesc,
        er.ExchangeRate,
        er.EffectiveDate,
        ROUND(t.PurchaseAmount * er.ExchangeRate, 2) AS ConvertedAmount
    FROM Transactions t
    CROSS APPLY (
        SELECT TOP 1
            CountryCurrencyDesc,
            ExchangeRate,
            EffectiveDate
        FROM ExchangeRates er
        WHERE er.EffectiveDate <= t.TransactionDate
            AND er.EffectiveDate >= DATEADD(MONTH, -6, t.TransactionDate)
        ORDER BY er.EffectiveDate DESC
    ) er
    ');
    PRINT 'TransactionCurrencyView created successfully.';
END
ELSE
BEGIN
    PRINT 'TransactionCurrencyView already exists.';
END
GO
