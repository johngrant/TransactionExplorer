import { useCallback, useEffect, useRef, useState } from "react";
import { ApiService, PagedResponse, Transaction } from "../services/api";

interface UseInfiniteTransactionsReturn {
  transactions: Transaction[];
  loading: boolean;
  error: string | null;
  hasMore: boolean;
  loadMore: () => void;
  refresh: () => void;
  addTransaction: (transaction: Transaction) => void;
  removeTransaction: (transactionId: number) => void;
}

export function useInfiniteTransactions(
  pageSize: number = 50
): UseInfiniteTransactionsReturn {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const initialLoad = useRef(true);

  const loadTransactions = useCallback(
    async (pageNumber: number, reset: boolean = false) => {
      if (loading) return;

      setLoading(true);
      setError(null);

      try {
        const response: PagedResponse<Transaction> =
          await ApiService.getTransactions({
            pageNumber,
            pageSize,
          });

        setTransactions((prev) =>
          reset ? response.items : [...prev, ...response.items]
        );
        setHasMore(response.hasNextPage);
        setCurrentPage(pageNumber);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to load transactions"
        );
      } finally {
        setLoading(false);
      }
    },
    [pageSize, loading]
  );

  // Initial load
  useEffect(() => {
    if (initialLoad.current) {
      initialLoad.current = false;
      loadTransactions(1, true);
    }
  }, [loadTransactions]);

  const loadMore = useCallback(() => {
    if (hasMore && !loading) {
      loadTransactions(currentPage + 1);
    }
  }, [hasMore, loading, currentPage, loadTransactions]);

  const refresh = useCallback(() => {
    setCurrentPage(1);
    setHasMore(true);
    loadTransactions(1, true);
  }, [loadTransactions]);

  const addTransaction = useCallback((transaction: Transaction) => {
    setTransactions((prev) => [transaction, ...prev]);
  }, []);

  const removeTransaction = useCallback((transactionId: number) => {
    setTransactions((prev) => prev.filter((t) => t.id !== transactionId));
  }, []);

  return {
    transactions,
    loading,
    error,
    hasMore,
    loadMore,
    refresh,
    addTransaction,
    removeTransaction,
  };
}
