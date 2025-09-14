-- Create the main database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TransactionExplorer')
BEGIN
    CREATE DATABASE TransactionExplorer;
    PRINT 'Database TransactionExplorer created successfully.';
END
ELSE
BEGIN
    PRINT 'Database TransactionExplorer already exists.';
END
GO
