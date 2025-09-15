import { useCallback, useEffect } from "react";

interface UseInfiniteScrollOptions {
  hasMore: boolean;
  loading: boolean;
  onLoadMore: () => void;
  threshold?: number; // How many pixels from bottom to trigger load
}

export function useInfiniteScroll({
  hasMore,
  loading,
  onLoadMore,
  threshold = 100,
}: UseInfiniteScrollOptions) {
  const handleScroll = useCallback(() => {
    if (loading || !hasMore) return;

    const scrollPosition = window.innerHeight + window.scrollY;
    const documentHeight = document.documentElement.scrollHeight;
    const remainingDistance = documentHeight - scrollPosition;

    if (remainingDistance <= threshold) {
      onLoadMore();
    }
  }, [loading, hasMore, onLoadMore, threshold]);

  useEffect(() => {
    window.addEventListener("scroll", handleScroll, { passive: true });

    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, [handleScroll]);
}
