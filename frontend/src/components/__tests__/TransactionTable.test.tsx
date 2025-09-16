import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import { Transaction } from '../../services/api';
import { TransactionTable } from '../TransactionTable';

// Mock react-router-dom
const mockNavigate = jest.fn();
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockNavigate,
}));

// Mock date utils
jest.mock('../../utils/dateUtils', () => ({
    formatTransactionDate: (date: string) => new Date(date).toLocaleDateString(),
}));

const mockTransactions: Transaction[] = [
    {
        id: 1,
        customId: 'TEST-001',
        description: 'Test Transaction 1',
        transactionDate: '2023-01-01T00:00:00Z',
        purchaseAmount: 100.50,
        createdAt: '2023-01-01T00:00:00Z',
        updatedAt: '2023-01-01T00:00:00Z'
    },
    {
        id: 2,
        customId: 'TEST-002',
        description: 'Another Transaction',
        transactionDate: '2023-01-02T00:00:00Z',
        purchaseAmount: 250.75,
        createdAt: '2023-01-02T00:00:00Z',
        updatedAt: '2023-01-02T00:00:00Z'
    },
    {
        id: 3,
        customId: 'TEST-003',
        description: 'Third Transaction',
        transactionDate: '2023-01-03T00:00:00Z',
        purchaseAmount: 75.25,
        createdAt: '2023-01-03T00:00:00Z',
        updatedAt: '2023-01-03T00:00:00Z'
    }
];

// Wrapper component for router context
const RouterWrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
    <BrowserRouter>{children}</BrowserRouter>
);

describe('TransactionTable', () => {
    const mockOnRefresh = jest.fn();
    const mockOnTransactionDeleted = jest.fn();

    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('renders transaction data correctly', () => {
        render(
            <RouterWrapper>
                <TransactionTable transactions={mockTransactions} />
            </RouterWrapper>
        );

        // Check if transactions are displayed
        expect(screen.getByText('TEST-001')).toBeInTheDocument();
        expect(screen.getByText('Test Transaction 1')).toBeInTheDocument();
        expect(screen.getByText('$100.50')).toBeInTheDocument();

        expect(screen.getByText('TEST-002')).toBeInTheDocument();
        expect(screen.getByText('Another Transaction')).toBeInTheDocument();
        expect(screen.getByText('$250.75')).toBeInTheDocument();
    });

    it('shows empty state when no transactions', () => {
        render(
            <RouterWrapper>
                <TransactionTable transactions={[]} />
            </RouterWrapper>
        );

        expect(screen.getByText('No transactions yet')).toBeInTheDocument();
    });

    it('filters transactions based on search term', async () => {
        const user = userEvent.setup();
        render(
            <RouterWrapper>
                <TransactionTable transactions={mockTransactions} />
            </RouterWrapper>
        );

        // Search for a specific description
        const searchInput = screen.getByPlaceholderText(/search transactions/i);
        await user.type(searchInput, 'Another');

        // Should only show matching transaction
        expect(screen.getByText('Another Transaction')).toBeInTheDocument();
        expect(screen.queryByText('Test Transaction 1')).not.toBeInTheDocument();
        expect(screen.queryByText('Third Transaction')).not.toBeInTheDocument();
    });

    it('filters transactions based on custom ID search', async () => {
        const user = userEvent.setup();
        render(
            <RouterWrapper>
                <TransactionTable transactions={mockTransactions} />
            </RouterWrapper>
        );

        // Search for a specific custom ID
        const searchInput = screen.getByPlaceholderText(/search transactions/i);
        await user.type(searchInput, 'TEST-003');

        // Should only show matching transaction
        expect(screen.getByText('Third Transaction')).toBeInTheDocument();
        expect(screen.queryByText('Test Transaction 1')).not.toBeInTheDocument();
        expect(screen.queryByText('Another Transaction')).not.toBeInTheDocument();
    });

    it('shows "no matches" message when search yields no results', async () => {
        const user = userEvent.setup();
        render(
            <RouterWrapper>
                <TransactionTable transactions={mockTransactions} />
            </RouterWrapper>
        );

        const searchInput = screen.getByPlaceholderText(/search transactions/i);
        await user.type(searchInput, 'nonexistent');

        expect(screen.getByText('No transactions match your search')).toBeInTheDocument();
    });



    it('navigates to transaction detail on row click', async () => {
        const user = userEvent.setup();
        render(
            <RouterWrapper>
                <TransactionTable transactions={mockTransactions} />
            </RouterWrapper>
        );

        // Click on the first transaction row
        const firstRow = screen.getByText('Test Transaction 1').closest('tr');
        if (firstRow) {
            await user.click(firstRow);
        }

        expect(mockNavigate).toHaveBeenCalledWith('/transaction/1');
    });

    it('shows refresh button when onRefresh prop is provided', () => {
        render(
            <RouterWrapper>
                <TransactionTable
                    transactions={mockTransactions}
                    onRefresh={mockOnRefresh}
                />
            </RouterWrapper>
        );

        const refreshButton = screen.getByRole('button', { name: /refresh/i });
        expect(refreshButton).toBeInTheDocument();
    });

    it('calls onRefresh when refresh button is clicked', async () => {
        const user = userEvent.setup();
        render(
            <RouterWrapper>
                <TransactionTable
                    transactions={mockTransactions}
                    onRefresh={mockOnRefresh}
                />
            </RouterWrapper>
        );

        const refreshButton = screen.getByRole('button', { name: /refresh/i });
        await user.click(refreshButton);

        expect(mockOnRefresh).toHaveBeenCalled();
    });

    it('disables refresh button when loading', () => {
        render(
            <RouterWrapper>
                <TransactionTable
                    transactions={mockTransactions}
                    onRefresh={mockOnRefresh}
                    loading={true}
                />
            </RouterWrapper>
        );

        const refreshButton = screen.getByRole('button', { name: /refresh/i });
        expect(refreshButton).toBeDisabled();
    });

    it('shows loading indicator when loading', () => {
        render(
            <RouterWrapper>
                <TransactionTable
                    transactions={mockTransactions}
                    loading={true}
                />
            </RouterWrapper>
        );

        expect(screen.getByText(/loading more transactions/i)).toBeInTheDocument();
    });

    it('shows end of transactions message when no more data', () => {
        render(
            <RouterWrapper>
                <TransactionTable
                    transactions={mockTransactions}
                    hasMore={false}
                />
            </RouterWrapper>
        );

        expect(screen.getByText(/end of transactions reached/i)).toBeInTheDocument();
    });

    it('does not show end message when there are more transactions', () => {
        render(
            <RouterWrapper>
                <TransactionTable
                    transactions={mockTransactions}
                    hasMore={true}
                />
            </RouterWrapper>
        );

        expect(screen.queryByText(/end of transactions reached/i)).not.toBeInTheDocument();
    });
});
