-- Migration: Update datetime2 columns to datetimeoffset to preserve timezone information
-- This ensures that Created and Updated timestamps maintain timezone context

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

    -- Add new columns with datetimeoffset type
    ALTER TABLE Transactions
    ADD CreatedAtOffset DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET();

    ALTER TABLE Transactions
    ADD UpdatedAtOffset DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET();
    GO

    -- Copy existing data, assuming UTC timezone for existing records
    UPDATE Transactions
    SET CreatedAtOffset = SWITCHOFFSET(CAST(CreatedAt AS DATETIMEOFFSET), '+00:00'),
        UpdatedAtOffset = SWITCHOFFSET(CAST(UpdatedAt AS DATETIMEOFFSET), '+00:00');
    GO

    -- Drop old columns
    ALTER TABLE Transactions DROP COLUMN CreatedAt;
    ALTER TABLE Transactions DROP COLUMN UpdatedAt;
    GO

    -- Rename new columns to original names
    EXEC sp_rename 'Transactions.CreatedAtOffset', 'CreatedAt', 'COLUMN';
    EXEC sp_rename 'Transactions.UpdatedAtOffset', 'UpdatedAt', 'COLUMN';
    GO

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

        -- Add new column with datetimeoffset type
        ALTER TABLE ExchangeRates
        ADD CreatedAtOffset DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET();
        GO

        -- Copy existing data, assuming UTC timezone for existing records
        UPDATE ExchangeRates
        SET CreatedAtOffset = SWITCHOFFSET(CAST(CreatedAt AS DATETIMEOFFSET), '+00:00');
        GO

        -- Drop old column
        ALTER TABLE ExchangeRates DROP COLUMN CreatedAt;
        GO

        -- Rename new column to original name
        EXEC sp_rename 'ExchangeRates.CreatedAtOffset', 'CreatedAt', 'COLUMN';
        GO

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
