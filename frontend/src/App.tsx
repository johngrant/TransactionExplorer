import { useState } from "react";
import { Header } from "./components/Header";
import { TransactionForm } from "./components/TransactionForm";
import { TransactionTable } from "./components/TransactionTable";
import { ThemeProvider } from "./components/ThemeProvider";

interface Transaction {
  id: string;
  description: string;
  date: string;
  amount: number;
}

export default function App() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);

  const handleAddTransaction = (transaction: Transaction) => {
    setTransactions(prev => [...prev, transaction]);
  };

  const existingIds = transactions.map(t => t.id);

  return (
    <ThemeProvider>
      <div className="min-h-screen bg-background">
        <Header />
        
        <main className="container mx-auto px-4 lg:px-6 py-6">
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
              <TransactionTable transactions={transactions} />
            </div>
          </div>
        </main>
      </div>
    </ThemeProvider>
  );
}