import { Loader2, RefreshCw, Search } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Transaction } from "../services/api";
import { formatTransactionDate } from "../utils/dateUtils";
import { Button } from "./ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";
import { Input } from "./ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "./ui/select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "./ui/table";

interface TransactionTableProps {
  transactions: Transaction[];
  loading?: boolean;
  hasMore?: boolean;
  onRefresh?: () => void;
  onTransactionDeleted?: (transactionId: number) => void;
}

export function TransactionTable({ transactions, loading = false, hasMore = false, onRefresh, onTransactionDeleted }: TransactionTableProps) {
  const [searchTerm, setSearchTerm] = useState("");
  const [sortField, setSortField] = useState<"date" | "amount" | "id" | "description">("date");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");
  const navigate = useNavigate();

  // Filter and sort transactions
  const filteredAndSortedTransactions = transactions
    .filter(transaction =>
      transaction.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      transaction.customId.toLowerCase().includes(searchTerm.toLowerCase())
    )
    .sort((a, b) => {
      let aValue: any;
      let bValue: any;

      switch (sortField) {
        case "date":
          aValue = new Date(a.transactionDate);
          bValue = new Date(b.transactionDate);
          break;
        case "amount":
          aValue = a.purchaseAmount;
          bValue = b.purchaseAmount;
          break;
        case "id":
          aValue = a.customId;
          bValue = b.customId;
          break;
        case "description":
          aValue = a.description.toLowerCase();
          bValue = b.description.toLowerCase();
          break;
        default:
          return 0;
      }

      if (aValue < bValue) return sortOrder === "asc" ? -1 : 1;
      if (aValue > bValue) return sortOrder === "asc" ? 1 : -1;
      return 0;
    });

  const toggleSortOrder = () => {
    setSortOrder(prev => prev === "asc" ? "desc" : "asc");
  };

  const handleRowClick = (transaction: Transaction) => {
    navigate(`/transaction/${transaction.id}`);
  };

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <CardTitle>Transactions</CardTitle>
          {onRefresh && (
            <Button
              variant="outline"
              size="sm"
              onClick={onRefresh}
              disabled={loading}
              className="flex items-center gap-2"
            >
              <RefreshCw className={`h-4 w-4 ${loading ? 'animate-spin' : ''}`} />
              Refresh
            </Button>
          )}
        </div>
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search transactions..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
          <div className="flex gap-2">
            <Select value={sortField} onValueChange={(value) => setSortField(value as "date" | "amount" | "id" | "description")}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Sort by">
                  <span>Sort: {
                    sortField === "date" ? "Date" :
                      sortField === "amount" ? "Amount" :
                        sortField === "description" ? "Description" :
                          sortField === "id" ? "Custom ID" : "Date"
                  }</span>
                </SelectValue>
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="date">Date</SelectItem>
                <SelectItem value="amount">Amount</SelectItem>
                <SelectItem value="description">Description</SelectItem>
                <SelectItem value="id">Custom ID</SelectItem>
              </SelectContent>
            </Select>
            <Button variant="outline" onClick={toggleSortOrder}>
              {sortOrder === "asc" ? "↑" : "↓"}
            </Button>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        <div className="overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Custom ID</TableHead>
                <TableHead>Description</TableHead>
                <TableHead>Date</TableHead>
                <TableHead className="text-right">Amount</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredAndSortedTransactions.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="text-center py-8 text-muted-foreground">
                    {transactions.length === 0 ? "No transactions yet" : "No transactions match your search"}
                  </TableCell>
                </TableRow>
              ) : (
                filteredAndSortedTransactions.map((transaction) => (
                  <TableRow
                    key={transaction.id}
                    className="cursor-pointer hover:bg-muted/50 transition-colors"
                    onClick={() => handleRowClick(transaction)}
                  >
                    <TableCell className="font-mono">{transaction.id}</TableCell>
                    <TableCell className="font-mono">{transaction.customId}</TableCell>
                    <TableCell>{transaction.description}</TableCell>
                    <TableCell>{formatTransactionDate(transaction.transactionDate)}</TableCell>
                    <TableCell className="text-right">${transaction.purchaseAmount.toFixed(2)}</TableCell>
                  </TableRow>
                ))
              )}
              {loading && (
                <TableRow>
                  <TableCell colSpan={5} className="text-center py-4">
                    <div className="flex items-center justify-center gap-2">
                      <Loader2 className="h-4 w-4 animate-spin" />
                      <span>Loading more transactions...</span>
                    </div>
                  </TableCell>
                </TableRow>
              )}
              {!hasMore && transactions.length > 0 && !loading && (
                <TableRow>
                  <TableCell colSpan={5} className="text-center py-4 text-muted-foreground">
                    <div className="flex items-center justify-center">
                      <span>End of transactions reached</span>
                    </div>
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  );
}
