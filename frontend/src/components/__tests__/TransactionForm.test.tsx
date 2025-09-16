import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ApiService } from '../../services/api';
import { TransactionForm } from '../TransactionForm';

// Mock the ApiService
jest.mock('../../services/api', () => ({
    ApiService: {
        createTransaction: jest.fn(),
    },
}));

const mockApiService = ApiService as jest.Mocked<typeof ApiService>;

describe('TransactionForm', () => {
    const mockOnAddTransaction = jest.fn();
    const existingIds = ['existing-id-1', 'existing-id-2'];

    beforeEach(() => {
        jest.clearAllMocks();
        mockApiService.createTransaction.mockResolvedValue({
            id: 1,
            customId: 'test-id',
            description: 'Test transaction',
            transactionDate: '2023-01-01',
            purchaseAmount: 100.00,
            createdAt: '2023-01-01T00:00:00Z',
            updatedAt: '2023-01-01T00:00:00Z'
        });
    });

    it('renders all form fields', () => {
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        expect(screen.getByLabelText(/description/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/transaction date/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/purchase amount/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/custom id/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /create transaction/i })).toBeInTheDocument();
    });

    it('shows validation errors for empty fields', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/description is required/i)).toBeInTheDocument();
            expect(screen.getByText(/transaction date must be a valid date format/i)).toBeInTheDocument();
            expect(screen.getByText(/purchase amount must be valid/i)).toBeInTheDocument();
            expect(screen.getByText(/custom id is required/i)).toBeInTheDocument();
        });
    });

    it('shows validation error for duplicate custom ID', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        const customIdInput = screen.getByLabelText(/custom id/i);
        await user.type(customIdInput, 'existing-id-1');

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/id must be unique/i)).toBeInTheDocument();
        });
    });

    it('shows validation error for description too long', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        const descriptionInput = screen.getByLabelText(/description/i);
        const longDescription = 'a'.repeat(51); // 51 characters
        await user.type(descriptionInput, longDescription);

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/description too long/i)).toBeInTheDocument();
        });
    });

    it('shows validation error for invalid amount', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        const amountInput = screen.getByLabelText(/purchase amount/i);
        await user.type(amountInput, '-10');

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/purchase amount must be valid/i)).toBeInTheDocument();
        });
    });

    it('successfully submits form with valid data', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        // Fill form with valid data
        await user.type(screen.getByLabelText(/description/i), 'Test transaction');
        await user.type(screen.getByLabelText(/transaction date/i), '2023-01-01');
        await user.type(screen.getByLabelText(/purchase amount/i), '100.50');
        await user.type(screen.getByLabelText(/custom id/i), 'unique-test-id');

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(mockApiService.createTransaction).toHaveBeenCalledWith({
                customId: 'unique-test-id',
                description: 'Test transaction',
                transactionDate: '2023-01-01',
                purchaseAmount: 100.50
            });
            expect(mockOnAddTransaction).toHaveBeenCalledWith({
                id: 1,
                customId: 'test-id',
                description: 'Test transaction',
                transactionDate: '2023-01-01',
                purchaseAmount: 100.00,
                createdAt: '2023-01-01T00:00:00Z',
                updatedAt: '2023-01-01T00:00:00Z'
            });
        });
    });

    it('shows loading state during submission', async () => {
        const user = userEvent.setup();
        // Make the API call take some time
        mockApiService.createTransaction.mockImplementation(() =>
            new Promise(resolve => setTimeout(() => resolve({
                id: 1,
                customId: 'test-id',
                description: 'Test transaction',
                transactionDate: '2023-01-01',
                purchaseAmount: 100.00,
                createdAt: '2023-01-01T00:00:00Z',
                updatedAt: '2023-01-01T00:00:00Z'
            }), 100))
        );

        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        // Fill form with valid data
        await user.type(screen.getByLabelText(/description/i), 'Test transaction');
        await user.type(screen.getByLabelText(/transaction date/i), '2023-01-01');
        await user.type(screen.getByLabelText(/purchase amount/i), '100.50');
        await user.type(screen.getByLabelText(/custom id/i), 'unique-test-id');

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        // Check loading state
        expect(screen.getByText(/creating.../i)).toBeInTheDocument();
        expect(submitButton).toBeDisabled();

        // Wait for completion
        await waitFor(() => {
            expect(screen.getByText(/create transaction/i)).toBeInTheDocument();
            expect(submitButton).not.toBeDisabled();
        });
    });

    it('clears errors when user starts typing', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        // Submit empty form to trigger errors
        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/description is required/i)).toBeInTheDocument();
        });

        // Start typing in description field
        const descriptionInput = screen.getByLabelText(/description/i);
        await user.type(descriptionInput, 'T');

        // Error should be cleared
        await waitFor(() => {
            expect(screen.queryByText(/description is required/i)).not.toBeInTheDocument();
        });
    });

    it('resets form after successful submission', async () => {
        const user = userEvent.setup();
        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        // Fill and submit form
        const descriptionInput = screen.getByLabelText(/description/i);
        const dateInput = screen.getByLabelText(/transaction date/i);
        const amountInput = screen.getByLabelText(/purchase amount/i);
        const customIdInput = screen.getByLabelText(/custom id/i);

        await user.type(descriptionInput, 'Test transaction');
        await user.type(dateInput, '2023-01-01');
        await user.type(amountInput, '100.50');
        await user.type(customIdInput, 'unique-test-id');

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(mockOnAddTransaction).toHaveBeenCalled();
        });

        // Form should be reset
        await waitFor(() => {
            expect((descriptionInput as HTMLInputElement).value).toBe('');
            expect((dateInput as HTMLInputElement).value).toBe('');
            expect((amountInput as HTMLInputElement).value).toBe('');
            expect((customIdInput as HTMLInputElement).value).toBe('');
        });
    });

    it('handles API errors gracefully', async () => {
        const user = userEvent.setup();
        mockApiService.createTransaction.mockRejectedValueOnce(new Error('Server error'));

        render(
            <TransactionForm
                onAddTransaction={mockOnAddTransaction}
                existingIds={existingIds}
            />
        );

        // Fill form with valid data
        await user.type(screen.getByLabelText(/description/i), 'Test transaction');
        await user.type(screen.getByLabelText(/transaction date/i), '2023-01-01');
        await user.type(screen.getByLabelText(/purchase amount/i), '100.50');
        await user.type(screen.getByLabelText(/custom id/i), 'unique-test-id');

        const submitButton = screen.getByRole('button', { name: /create transaction/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(screen.getByText(/server error/i)).toBeInTheDocument();
        });

        expect(mockOnAddTransaction).not.toHaveBeenCalled();
    });
});
