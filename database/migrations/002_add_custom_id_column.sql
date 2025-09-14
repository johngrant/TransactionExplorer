-- Migration: Add CustomId column to Transactions table
-- Date: 2025-09-14
-- Description: Adds a CustomId NVARCHAR(100) column with unique constraint to the Transactions table

-- Add the CustomId column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'CustomId')
BEGIN
    ALTER TABLE Transactions
    ADD CustomId NVARCHAR(100);

    PRINT 'CustomId column added to Transactions table.';
END
ELSE
BEGIN
    PRINT 'CustomId column already exists in Transactions table.';
END
GO

-- Add unique constraint on CustomId column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
               WHERE CONSTRAINT_NAME = 'UK_Transactions_CustomId'
               AND TABLE_NAME = 'Transactions')
BEGIN
    -- First, populate existing records with unique CustomId values
    -- Using a combination of 'TXN-' prefix and Id to ensure uniqueness
    UPDATE Transactions
    SET CustomId = 'TXN-' + CAST(Id AS NVARCHAR(10))
    WHERE CustomId IS NULL;

    -- Make the column NOT NULL
    ALTER TABLE Transactions
    ALTER COLUMN CustomId NVARCHAR(100) NOT NULL;

    -- Add the unique constraint
    ALTER TABLE Transactions
    ADD CONSTRAINT UK_Transactions_CustomId UNIQUE (CustomId);

    PRINT 'Unique constraint UK_Transactions_CustomId added to Transactions table.';
END
ELSE
BEGIN
    PRINT 'Unique constraint UK_Transactions_CustomId already exists.';
END
GO

PRINT 'Migration 002_add_custom_id_column completed successfully.';
GO
