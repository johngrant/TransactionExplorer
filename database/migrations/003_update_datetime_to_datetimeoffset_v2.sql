-- Migration: Update datetime2 columns to datetimeoffset to preserve timezone information
-- This approach drops and recreates the table to avoid constraint conflicts

USE [TransactionExplorer];
GO

PRINT 'Starting migration 003: Update datetime2 to datetimeoffset';

-- Check if the migration has already been applied
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Transactions'
    AND COLUMN_NAME = 'CreatedAt'
    AND DATA_TYPE = 'datetimeoffset'
)
BEGIN
    PRINT 'Updating Transactions table to use DATETIMEOFFSET...';

    -- Create backup table with existing data
    SELECT * INTO TransactionsBackup FROM Transactions;

    -- Drop the original table
    DROP TABLE Transactions;

    -- Recreate table with datetimeoffset columns
    CREATE TABLE Transactions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomId NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(50) NOT NULL,
        TransactionDate DATE NOT NULL,
        PurchaseAmount DECIMAL(19,2) NOT NULL CHECK (PurchaseAmount > 0),
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );

    -- Restore data with timezone conversion (assuming original data is UTC)
    SET IDENTITY_INSERT Transactions ON;

    INSERT INTO Transactions (Id, CustomId, Description, TransactionDate, PurchaseAmount, CreatedAt, UpdatedAt)
    SELECT
        Id,
        CustomId,
        Description,
        TransactionDate,
        PurchaseAmount,
        SWITCHOFFSET(CAST(CreatedAt AS DATETIMEOFFSET), '+00:00') AS CreatedAt,
        SWITCHOFFSET(CAST(UpdatedAt AS DATETIMEOFFSET), '+00:00') AS UpdatedAt
    FROM TransactionsBackup;

    SET IDENTITY_INSERT Transactions OFF;

    -- Drop backup table
    DROP TABLE TransactionsBackup;

    PRINT 'Transactions table updated successfully.';
END
ELSE
BEGIN
    PRINT 'Transactions table already uses DATETIMEOFFSET.';
END
GO

-- Update ExchangeRates table if it exists
IF EXISTS (SELECT * FROM sysobjects WHERE name='ExchangeRates' AND xtype='U')
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'ExchangeRates'
        AND COLUMN_NAME = 'CreatedAt'
        AND DATA_TYPE = 'datetimeoffset'
    )
    BEGIN
        PRINT 'Updating ExchangeRates table to use DATETIMEOFFSET...';

        -- Create backup table with existing data
        SELECT * INTO ExchangeRatesBackup FROM ExchangeRates;

        -- Drop the original table
        DROP TABLE ExchangeRates;

        -- Recreate table with datetimeoffset column
        CREATE TABLE ExchangeRates (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            RecordDate DATE NOT NULL,
            CountryCurrencyDesc NVARCHAR(100) NOT NULL,
            ExchangeRate DECIMAL(19,6) NOT NULL,
            EffectiveDate DATE NOT NULL,
            CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
        );

        -- Create indexes
        CREATE NONCLUSTERED INDEX IX_ExchangeRates_Currency_Date
        ON ExchangeRates (CountryCurrencyDesc, EffectiveDate DESC);

        CREATE NONCLUSTERED INDEX IX_ExchangeRates_EffectiveDate
        ON ExchangeRates (EffectiveDate DESC);

        -- Restore data with timezone conversion (assuming original data is UTC)
        SET IDENTITY_INSERT ExchangeRates ON;

        INSERT INTO ExchangeRates (Id, RecordDate, CountryCurrencyDesc, ExchangeRate, EffectiveDate, CreatedAt)
        SELECT
            Id,
            RecordDate,
            CountryCurrencyDesc,
            ExchangeRate,
            EffectiveDate,
            SWITCHOFFSET(CAST(CreatedAt AS DATETIMEOFFSET), '+00:00') AS CreatedAt
        FROM ExchangeRatesBackup;

        SET IDENTITY_INSERT ExchangeRates OFF;

        -- Drop backup table
        DROP TABLE ExchangeRatesBackup;

        PRINT 'ExchangeRates table updated successfully.';
    END
    ELSE
    BEGIN
        PRINT 'ExchangeRates table already uses DATETIMEOFFSET.';
    END
END
GO

PRINT 'Migration 003 completed successfully.';
GO
