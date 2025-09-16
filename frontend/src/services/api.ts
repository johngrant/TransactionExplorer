const API_BASE_URL = "https://localhost:7237/api";

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

export interface CurrencyConversionResponse {
  originalAmountUsd: number;
  transactionDate: string;
  exchangeRate: number;
  convertedAmount: number;
  targetCurrency: string;
  exchangeRateDate: string;
  isExactDateMatch: boolean;
}

export class ApiService {
  static async getTransactions(
    params: PaginationParameters
  ): Promise<PagedResponse<Transaction>> {
    const url = new URL(`${API_BASE_URL}/transactions/paged`);
    url.searchParams.append("pageNumber", params.pageNumber.toString());
    url.searchParams.append("pageSize", params.pageSize.toString());

    const response = await fetch(url.toString());

    if (!response.ok) {
      throw new Error(`Failed to fetch transactions: ${response.statusText}`);
    }

    return response.json();
  }

  static async getTransaction(id: number): Promise<Transaction> {
    const response = await fetch(`${API_BASE_URL}/transactions/${id}`);

    if (!response.ok) {
      throw new Error(`Failed to get transaction: ${response.statusText}`);
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
      let errorMessage = `Failed to create transaction: ${response.statusText}`;

      try {
        // Try to get detailed error information from the response body
        const errorBody = await response.json();

        if (errorBody.errors) {
          // ASP.NET Core validation errors format
          const validationErrors = Object.values(errorBody.errors).flat();
          errorMessage = validationErrors.join(", ");
        } else if (errorBody.title) {
          errorMessage = errorBody.title;
        }
      } catch {
        // If we can't parse the error body, use the status text
      }

      throw new Error(errorMessage);
    }

    return response.json();
  }

  static async deleteTransaction(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/transactions/${id}`, {
      method: "DELETE",
    });

    if (!response.ok) {
      throw new Error(`Failed to delete transaction: ${response.statusText}`);
    }
  }

  static async convertCurrency(
    transactionDate: string,
    amountUsd: number,
    countryCurrencyDesc: string
  ): Promise<CurrencyConversionResponse> {
    const url = new URL(`${API_BASE_URL}/exchangerate/convert`);
    url.searchParams.append("transactionDate", transactionDate);
    url.searchParams.append("amountUsd", amountUsd.toString());
    url.searchParams.append("countryCurrencyDesc", countryCurrencyDesc);

    const response = await fetch(url.toString());

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error("Exchange Rate Not Found");
      }

      // Handle server errors that might contain the "exchange rate not found" message
      if (response.status === 500) {
        try {
          const errorText = await response.text();
          if (
            errorText.toLowerCase().includes("no exchange rate found") ||
            errorText.toLowerCase().includes("exchange rate not found")
          ) {
            throw new Error("Exchange Rate Not Found");
          }
        } catch {
          // If we can't parse the error, fall through to the generic error
        }
      }

      throw new Error(`Failed to convert currency: ${response.statusText}`);
    }

    return response.json();
  }
}
