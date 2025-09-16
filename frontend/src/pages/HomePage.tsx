import { ErrorDisplay } from "../components/ErrorDisplay";
import { TransactionForm } from "../components/TransactionForm";
import { TransactionTable } from "../components/TransactionTable";
import { useInfiniteScroll } from "../hooks/useInfiniteScroll";
import { useInfiniteTransactions } from "../hooks/useInfiniteTransactions";
import { Transaction } from "../services/api";

export function HomePage() {
    const {
        transactions,
        loading,
        error,
        hasMore,
        loadMore,
        refresh,
        addTransaction,
        removeTransaction,
    } = useInfiniteTransactions(50);

    useInfiniteScroll({
        hasMore,
        loading,
        onLoadMore: loadMore,
        threshold: 200,
    });

    const handleAddTransaction = (transaction: Transaction) => {
        addTransaction(transaction);
    };

    const handleTransactionDeleted = (transactionId: number) => {
        removeTransaction(transactionId);
    };

    const existingIds = transactions.map(t => t.customId);

    return (
        <main className="container mx-auto px-4 lg:px-6 py-6">
            {error && (
                <div className="mb-6">
                    <ErrorDisplay
                        error={error}
                        onRetry={refresh}
                        title="Failed to load transactions"
                    />
                </div>
            )}

            <div className="grid grid-cols-1 gap-8">
                {/* Transaction Form - Top Half */}
                <div>
                    <TransactionForm
                        onAddTransaction={handleAddTransaction}
                        existingIds={existingIds}
                    />
                </div>

                {/* Transaction Table - Bottom Half */}
                <div>
                    <TransactionTable
                        transactions={transactions}
                        loading={loading}
                        hasMore={hasMore}
                        onRefresh={refresh}
                        onTransactionDeleted={handleTransactionDeleted}
                    />
                </div>
            </div>
        </main>
    );
}
