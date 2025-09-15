import { ErrorDisplay } from "./components/ErrorDisplay";
import { Header } from "./components/Header";
import { ThemeProvider } from "./components/ThemeProvider";
import { TransactionForm } from "./components/TransactionForm";
import { TransactionTable } from "./components/TransactionTable";
import { useInfiniteScroll } from "./hooks/useInfiniteScroll";
import { useInfiniteTransactions } from "./hooks/useInfiniteTransactions";
import { Transaction } from "./services/api";

export default function App() {
  const {
    transactions,
    loading,
    error,
    hasMore,
    loadMore,
    refresh,
    addTransaction,
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

  const existingIds = transactions.map(t => t.customId);

  return (
    <ThemeProvider>
      <div className="min-h-screen bg-background">
        <Header />

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
              />
            </div>
          </div>
        </main>
      </div>
    </ThemeProvider>
  );
}
