import { ArrowLeft, ArrowLeftRight, Calendar, Check, ChevronsUpDown, Clock, DollarSign, FileText, Hash, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Badge } from "../components/ui/badge";
import { Button } from "../components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "../components/ui/command";
import { LoadingSpinner } from "../components/ui/loading-spinner";
import { Popover, PopoverContent, PopoverTrigger } from "../components/ui/popover";
import { Separator } from "../components/ui/separator";
import { ApiService, Transaction } from "../services/api";
import { formatDateTime, formatTransactionDateDetailed } from "../utils/dateUtils";

// Dummy currencies for demonstration
const CURRENCIES = [
    { code: "EUR", name: "Euro", symbol: "€", rate: 0.85 },
    { code: "GBP", name: "British Pound", symbol: "£", rate: 0.73 },
    { code: "JPY", name: "Japanese Yen", symbol: "¥", rate: 110.25 },
    { code: "CAD", name: "Canadian Dollar", symbol: "C$", rate: 1.25 },
    { code: "AUD", name: "Australian Dollar", symbol: "A$", rate: 1.35 },
    { code: "CHF", name: "Swiss Franc", symbol: "CHF", rate: 0.92 },
    { code: "CNY", name: "Chinese Yuan", symbol: "¥", rate: 6.45 },
    { code: "SEK", name: "Swedish Krona", symbol: "kr", rate: 8.95 },
    { code: "NOK", name: "Norwegian Krone", symbol: "kr", rate: 8.75 },
    { code: "DKK", name: "Danish Krone", symbol: "kr", rate: 6.35 },
];

export function TransactionDetailsPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [transaction, setTransaction] = useState<Transaction | null>(null);
    const [loading, setLoading] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [selectedCurrency, setSelectedCurrency] = useState<string>("");
    const [open, setOpen] = useState(false);

    // Load transaction details when component mounts
    useEffect(() => {
        if (id) {
            loadTransaction(parseInt(id));
        }
    }, [id]);

    const loadTransaction = async (transactionId: number) => {
        setLoading(true);
        setError(null);

        try {
            const foundTransaction = await ApiService.getTransaction(transactionId);
            setTransaction(foundTransaction);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to load transaction");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async () => {
        if (!transaction?.id) return;

        setDeleting(true);
        try {
            await ApiService.deleteTransaction(transaction.id);
            navigate('/'); // Navigate back to home after deletion
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to delete transaction");
        } finally {
            setDeleting(false);
        }
    };

    const handleGoBack = () => {
        navigate('/');
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-background">
                <div className="container mx-auto px-4 lg:px-6 py-6">
                    <div className="flex items-center justify-center py-12">
                        <LoadingSpinner text="Loading transaction details..." size="lg" />
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen bg-background">
                <div className="container mx-auto px-4 lg:px-6 py-6">
                    <div className="mb-6">
                        <Button variant="outline" onClick={handleGoBack} className="flex items-center gap-2">
                            <ArrowLeft className="h-4 w-4" />
                            Back to Transactions
                        </Button>
                    </div>
                    <Card>
                        <CardContent className="flex flex-col items-center justify-center py-12 text-center">
                            <p className="text-destructive mb-4">{error}</p>
                            <div className="flex gap-2">
                                <Button variant="outline" onClick={handleGoBack}>
                                    Back to Transactions
                                </Button>
                                <Button onClick={() => id && loadTransaction(parseInt(id))}>
                                    Retry
                                </Button>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        );
    }

    if (!transaction) {
        return (
            <div className="min-h-screen bg-background">
                <div className="container mx-auto px-4 lg:px-6 py-6">
                    <div className="mb-6">
                        <Button variant="outline" onClick={handleGoBack} className="flex items-center gap-2">
                            <ArrowLeft className="h-4 w-4" />
                            Back to Transactions
                        </Button>
                    </div>
                    <Card>
                        <CardContent className="flex items-center justify-center py-12">
                            <p className="text-muted-foreground">Transaction not found</p>
                        </CardContent>
                    </Card>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-background">
            <div className="container mx-auto px-4 lg:px-6 py-6">
                {/* Back Button */}
                <div className="mb-6">
                    <Button variant="outline" onClick={handleGoBack} className="flex items-center gap-2">
                        <ArrowLeft className="h-4 w-4" />
                        Back to Transactions
                    </Button>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    {/* Main Transaction Details */}
                    <div className="lg:col-span-2">
                        <Card>
                            <CardHeader>
                                <CardTitle>Transaction Details</CardTitle>
                            </CardHeader>
                            <CardContent className="space-y-6">
                                {/* Transaction Header */}
                                <div className="flex items-start justify-between">
                                    <div className="space-y-1">
                                        <div className="flex items-center gap-2">
                                            <Hash className="h-4 w-4 text-muted-foreground" />
                                            <span className="font-mono text-sm text-muted-foreground">ID: {transaction.id}</span>
                                        </div>
                                        <div className="flex items-center gap-2">
                                            <Badge variant="secondary" className="font-mono">
                                                {transaction.customId}
                                            </Badge>
                                        </div>
                                    </div>
                                    <div className="text-right">
                                        <div className="text-3xl font-bold text-green-600">
                                            ${transaction.purchaseAmount.toFixed(2)}
                                        </div>
                                    </div>
                                </div>

                                <Separator />

                                {/* Transaction Details */}
                                <div className="space-y-4">
                                    <div className="flex items-start gap-3">
                                        <FileText className="h-5 w-5 text-muted-foreground mt-0.5" />
                                        <div className="flex-1">
                                            <p className="font-medium text-sm text-muted-foreground mb-1">Description</p>
                                            <p className="text-foreground">{transaction.description}</p>
                                        </div>
                                    </div>

                                    <div className="flex items-center gap-3">
                                        <Calendar className="h-5 w-5 text-muted-foreground" />
                                        <div className="flex-1">
                                            <p className="font-medium text-sm text-muted-foreground mb-1">Transaction Date</p>
                                            <p className="text-foreground">{formatTransactionDateDetailed(transaction.transactionDate)}</p>
                                        </div>
                                    </div>

                                    <div className="flex items-center gap-3">
                                        <DollarSign className="h-5 w-5 text-muted-foreground" />
                                        <div className="flex-1">
                                            <p className="font-medium text-sm text-muted-foreground mb-1">Amount</p>
                                            <p className="text-foreground font-mono">${transaction.purchaseAmount.toFixed(2)}</p>
                                        </div>
                                    </div>
                                </div>

                                <Separator />

                                {/* Metadata */}
                                <div className="space-y-3">
                                    <h4 className="font-medium text-sm text-muted-foreground">Metadata</h4>
                                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm">
                                        <div className="flex items-center gap-2">
                                            <Clock className="h-4 w-4 text-muted-foreground" />
                                            <div>
                                                <p className="text-muted-foreground">Created</p>
                                                <p className="font-mono">{formatDateTime(transaction.createdAt)}</p>
                                            </div>
                                        </div>
                                        <div className="flex items-center gap-2">
                                            <Clock className="h-4 w-4 text-muted-foreground" />
                                            <div>
                                                <p className="text-muted-foreground">Last Updated</p>
                                                <p className="font-mono">{formatDateTime(transaction.updatedAt)}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                {/* Actions */}
                                <div className="flex justify-end gap-2 pt-4 border-t">
                                    <Button variant="outline" onClick={handleGoBack}>
                                        Close
                                    </Button>
                                    <Button
                                        variant="destructive"
                                        onClick={handleDelete}
                                        disabled={deleting}
                                        className="flex items-center gap-2"
                                    >
                                        {deleting ? (
                                            <>
                                                <LoadingSpinner text="Deleting..." size="sm" />
                                            </>
                                        ) : (
                                            <>
                                                <Trash2 className="h-4 w-4" />
                                                Delete
                                            </>
                                        )}
                                    </Button>
                                </div>
                            </CardContent>
                        </Card>
                    </div>

                    {/* Currency Conversion Sidebar */}
                    <div className="lg:col-span-1">
                        <Card>
                            <CardHeader>
                                <CardTitle className="flex items-center gap-2">
                                    <ArrowLeftRight className="h-5 w-5" />
                                    Currency Conversion
                                </CardTitle>
                            </CardHeader>
                            <CardContent className="space-y-4">
                                <div>
                                    <p className="text-sm text-muted-foreground mb-2">Convert to:</p>
                                    <Popover open={open} onOpenChange={setOpen}>
                                        <PopoverTrigger asChild>
                                            <Button
                                                variant="outline"
                                                role="combobox"
                                                aria-expanded={open}
                                                className="w-full justify-between"
                                            >
                                                {selectedCurrency
                                                    ? CURRENCIES.find((currency) => currency.code === selectedCurrency)?.name +
                                                    ` (${CURRENCIES.find((currency) => currency.code === selectedCurrency)?.code})`
                                                    : "Select currency..."
                                                }
                                                <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                                            </Button>
                                        </PopoverTrigger>
                                        <PopoverContent className="w-[280px] p-0">
                                            <Command>
                                                <CommandInput placeholder="Search currency..." />
                                                <CommandList>
                                                    <CommandEmpty>No currency found.</CommandEmpty>
                                                    <CommandGroup>
                                                        {CURRENCIES.map((currency) => (
                                                            <CommandItem
                                                                key={currency.code}
                                                                value={currency.code}
                                                                onSelect={(currentValue) => {
                                                                    setSelectedCurrency(currentValue === selectedCurrency ? "" : currentValue);
                                                                    setOpen(false);
                                                                }}
                                                            >
                                                                <Check
                                                                    className={`mr-2 h-4 w-4 ${selectedCurrency === currency.code ? "opacity-100" : "opacity-0"
                                                                        }`}
                                                                />
                                                                <div className="flex items-center justify-between w-full">
                                                                    <span className="flex items-center gap-2">
                                                                        <span className="font-medium">{currency.symbol}</span>
                                                                        <span>{currency.name}</span>
                                                                    </span>
                                                                    <Badge variant="secondary" className="ml-2">
                                                                        {currency.code}
                                                                    </Badge>
                                                                </div>
                                                            </CommandItem>
                                                        ))}
                                                    </CommandGroup>
                                                </CommandList>
                                            </Command>
                                        </PopoverContent>
                                    </Popover>
                                </div>

                                {selectedCurrency && (
                                    <div className="bg-muted/50 rounded-lg p-4 space-y-3">
                                        <div className="flex items-center justify-between text-sm">
                                            <span className="text-muted-foreground">Exchange Rate (USD → {selectedCurrency}):</span>
                                            <span className="font-mono">
                                                {CURRENCIES.find(c => c.code === selectedCurrency)?.rate?.toFixed(4)}
                                            </span>
                                        </div>
                                        <Separator />
                                        <div className="flex items-center justify-between">
                                            <span className="font-medium text-sm text-muted-foreground">Converted Amount:</span>
                                            <div className="text-right">
                                                <div className="text-lg font-bold text-green-600">
                                                    {CURRENCIES.find(c => c.code === selectedCurrency)?.symbol}
                                                    {((transaction.purchaseAmount * (CURRENCIES.find(c => c.code === selectedCurrency)?.rate || 1))).toFixed(2)}
                                                </div>
                                                <div className="text-xs text-muted-foreground">
                                                    {CURRENCIES.find(c => c.code === selectedCurrency)?.code}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                )}
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
}
