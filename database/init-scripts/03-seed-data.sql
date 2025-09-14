-- Seed data for Transaction Explorer application

-- First, insert some exchange rate data for common currencies
-- This data is based on the Treasury Reporting Rates format

INSERT INTO ExchangeRates (RecordDate, CountryCurrencyDesc, ExchangeRate, EffectiveDate) VALUES
-- EUR rates for different dates
('2024-12-31', 'Euro Zone-Euro', 0.95, '2024-12-31'),
('2024-11-30', 'Euro Zone-Euro', 0.94, '2024-11-30'),
('2024-10-31', 'Euro Zone-Euro', 0.93, '2024-10-31'),
('2024-09-30', 'Euro Zone-Euro', 0.92, '2024-09-30'),

-- GBP rates
('2024-12-31', 'United Kingdom-Pound', 0.78, '2024-12-31'),
('2024-11-30', 'United Kingdom-Pound', 0.79, '2024-11-30'),
('2024-10-31', 'United Kingdom-Pound', 0.80, '2024-10-31'),
('2024-09-30', 'United Kingdom-Pound', 0.81, '2024-09-30'),

-- CAD rates
('2024-12-31', 'Canada-Dollar', 1.35, '2024-12-31'),
('2024-11-30', 'Canada-Dollar', 1.34, '2024-11-30'),
('2024-10-31', 'Canada-Dollar', 1.33, '2024-10-31'),
('2024-09-30', 'Canada-Dollar', 1.32, '2024-09-30'),

-- JPY rates
('2024-12-31', 'Japan-Yen', 150.25, '2024-12-31'),
('2024-11-30', 'Japan-Yen', 149.80, '2024-11-30'),
('2024-10-31', 'Japan-Yen', 148.90, '2024-10-31'),
('2024-09-30', 'Japan-Yen', 147.50, '2024-09-30'),

-- AUD rates
('2024-12-31', 'Australia-Dollar', 1.52, '2024-12-31'),
('2024-11-30', 'Australia-Dollar', 1.51, '2024-11-30'),
('2024-10-31', 'Australia-Dollar', 1.50, '2024-10-31'),
('2024-09-30', 'Australia-Dollar', 1.49, '2024-09-30'),

-- Recent rates for 2025
('2025-06-30', 'Euro Zone-Euro', 0.91, '2025-06-30'),
('2025-06-30', 'United Kingdom-Pound', 0.77, '2025-06-30'),
('2025-06-30', 'Canada-Dollar', 1.38, '2025-06-30'),
('2025-06-30', 'Japan-Yen', 155.20, '2025-06-30'),
('2025-06-30', 'Australia-Dollar', 1.55, '2025-06-30');

PRINT 'Exchange rates seeded successfully.';
GO

-- Insert 20 sample transaction records
INSERT INTO Transactions (Description, TransactionDate, PurchaseAmount) VALUES
('Office Supplies - Stapler and Paper', '2024-12-15', 45.67),
('Business Lunch - Client Meeting', '2024-12-14', 125.50),
('Software License - Productivity Suite', '2024-12-13', 299.99),
('Travel Expense - Airport Parking', '2024-12-12', 35.00),
('Office Equipment - Wireless Mouse', '2024-12-11', 28.99),
('Professional Development Book', '2024-12-10', 55.75),
('Conference Registration Fee', '2024-12-09', 450.00),
('Business Cards Printing', '2024-12-08', 89.25),
('Cloud Storage Subscription', '2024-12-07', 15.99),
('Office Coffee Supply', '2024-12-06', 75.30),
('Marketing Materials - Brochures', '2024-11-25', 180.45),
('Team Building Event Catering', '2024-11-20', 320.80),
('Internet Service - Monthly Fee', '2024-11-15', 99.95),
('Training Workshop Attendance', '2024-11-10', 275.00),
('Office Furniture - Desk Chair', '2024-11-05', 189.99),
('Business Phone Service', '2024-10-30', 125.00),
('Computer Monitor Purchase', '2024-10-25', 399.99),
('Office Cleaning Service', '2024-10-20', 150.00),
('Professional Consultation Fee', '2024-10-15', 500.00),
('Presentation Equipment - Projector', '2024-10-10', 699.99);

PRINT 'Transaction records seeded successfully.';
GO

-- Display summary of seeded data
SELECT 'Transactions' as TableName, COUNT(*) as RecordCount FROM Transactions
UNION ALL
SELECT 'ExchangeRates' as TableName, COUNT(*) as RecordCount FROM ExchangeRates;

PRINT 'Database seeding completed successfully.';
GO
