const API_BASE_URL = "http://localhost:5070/api";

export interface Transaction {
  id: number;
  customId: string;
  description: string;
  transactionDate: string;
  purchaseAmount: number;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResponse<T> {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  items: T[];
}

export interface PaginationParameters {
  pageNumber: number;
  pageSize: number;
}

export class ApiService {
  static async getTransactions(params: PaginationParameters): Promise<PagedResponse<Transaction>> {
    const url = new URL(`${API_BASE_URL}/transactions/paginated`);
    url.searchParams.append('pageNumber', params.pageNumber.toString());
    url.searchParams.append('pageSize', params.pageSize.toString());

    const response = await fetch(url.toString());

    if (!response.ok) {
      throw new Error(`Failed to fetch transactions: ${response.statusText}`);
    }

    return response.json();
  }

  static async createTransaction(transaction: {
    customId: string;
    description: string;
    transactionDate: string;
    purchaseAmount: number;
  }): Promise<Transaction> {
    const response = await fetch(`${API_BASE_URL}/transactions`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(transaction),
    });

    if (!response.ok) {
      throw new Error(`Failed to create transaction: ${response.statusText}`);
    }

    return response.json();
  }
}
