-- Database migration script for Transaction Explorer
-- This script can be run from .NET Entity Framework migrations or standalone

-- Enable SQLCMD mode for variable usage
:setvar DatabaseName "TransactionExplorer"

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$(DatabaseName)')
BEGIN
    CREATE DATABASE [$(DatabaseName)];
    PRINT 'Database $(DatabaseName) created.';
END
ELSE
BEGIN
    PRINT 'Database $(DatabaseName) already exists.';
END
GO

USE [$(DatabaseName)];
GO

-- Create Transactions table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Transactions')
BEGIN
    CREATE TABLE [dbo].[Transactions] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Description] nvarchar(50) NOT NULL,
        [TransactionDate] date NOT NULL,
        [PurchaseAmount] decimal(19,2) NOT NULL,
        [CreatedAt] datetime2(7) NOT NULL DEFAULT (getutcdate()),
        [UpdatedAt] datetime2(7) NOT NULL DEFAULT (getutcdate()),
        
        CONSTRAINT [CK_Transactions_PurchaseAmount] CHECK ([PurchaseAmount] > 0)
    );
    
    PRINT 'Transactions table created.';
END
GO

-- Create ExchangeRates table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ExchangeRates')
BEGIN
    CREATE TABLE [dbo].[ExchangeRates] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RecordDate] date NOT NULL,
        [CountryCurrencyDesc] nvarchar(100) NOT NULL,
        [ExchangeRate] decimal(19,6) NOT NULL,
        [EffectiveDate] date NOT NULL,
        [CreatedAt] datetime2(7) NOT NULL DEFAULT (getutcdate())
    );
    
    -- Create indexes for efficient lookups
    CREATE NONCLUSTERED INDEX [IX_ExchangeRates_Currency_Date] 
    ON [dbo].[ExchangeRates] ([CountryCurrencyDesc], [EffectiveDate] DESC);
    
    CREATE NONCLUSTERED INDEX [IX_ExchangeRates_EffectiveDate] 
    ON [dbo].[ExchangeRates] ([EffectiveDate] DESC);
    
    PRINT 'ExchangeRates table created with indexes.';
END
GO

-- Create update trigger for Transactions
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Transactions_UpdatedAt')
BEGIN
    EXEC ('
    CREATE TRIGGER [dbo].[TR_Transactions_UpdatedAt]
    ON [dbo].[Transactions]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        
        UPDATE t
        SET UpdatedAt = GETUTCDATE()
        FROM [dbo].[Transactions] t
        INNER JOIN inserted i ON t.Id = i.Id;
    END
    ');
    
    PRINT 'Transactions UpdatedAt trigger created.';
END
GO

PRINT 'Database migration completed successfully.';
GO
