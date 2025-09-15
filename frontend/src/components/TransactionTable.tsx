import { Filter, Loader2, RefreshCw, Search } from "lucide-react";
import { useState } from "react";
import { Transaction } from "../services/api";
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
}

export function TransactionTable({ transactions, loading = false, hasMore = false, onRefresh }: TransactionTableProps) {
  const [searchTerm, setSearchTerm] = useState("");
  const [sortBy, setSortBy] = useState("transactionDate");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");

  const filteredAndSortedTransactions = transactions
    .filter(transaction =>
      transaction.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      transaction.customId.toLowerCase().includes(searchTerm.toLowerCase()) ||
      transaction.id.toString().includes(searchTerm)
    )
    .sort((a, b) => {
      let aValue: string | number;
      let bValue: string | number;

      switch (sortBy) {
        case "transactionDate":
          aValue = new Date(a.transactionDate).getTime();
          bValue = new Date(b.transactionDate).getTime();
          break;
        case "purchaseAmount":
          aValue = a.purchaseAmount;
          bValue = b.purchaseAmount;
          break;
        case "description":
          aValue = a.description.toLowerCase();
          bValue = b.description.toLowerCase();
          break;
        case "customId":
          aValue = a.customId.toLowerCase();
          bValue = b.customId.toLowerCase();
          break;
        case "id":
          aValue = a.id;
          bValue = b.id;
          break;
        default:
          aValue = new Date(a.transactionDate).getTime();
          bValue = new Date(b.transactionDate).getTime();
      }

      if (sortOrder === "asc") {
        return aValue < bValue ? -1 : aValue > bValue ? 1 : 0;
      } else {
        return aValue > bValue ? -1 : aValue < bValue ? 1 : 0;
      }
    });

  const toggleSortOrder = () => {
    setSortOrder(prev => prev === "asc" ? "desc" : "asc");
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
            <Select value={sortBy} onValueChange={setSortBy}>
              <SelectTrigger className="w-40">
                <Filter className="h-4 w-4 mr-2" />
                <SelectValue placeholder="Sort by" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="transactionDate">Date</SelectItem>
                <SelectItem value="purchaseAmount">Amount</SelectItem>
                <SelectItem value="description">Description</SelectItem>
                <SelectItem value="customId">Custom ID</SelectItem>
                <SelectItem value="id">ID</SelectItem>
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
                  <TableRow key={transaction.id}>
                    <TableCell className="font-mono">{transaction.id}</TableCell>
                    <TableCell className="font-mono">{transaction.customId}</TableCell>
                    <TableCell>{transaction.description}</TableCell>
                    <TableCell>{new Date(transaction.transactionDate).toLocaleDateString()}</TableCell>
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
