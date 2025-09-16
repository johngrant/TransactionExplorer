import { ArrowLeft, ArrowLeftRight, Calendar, Check, ChevronsUpDown, Clock, DollarSign, FileText, Hash, Trash2 } from "lucide-react";
import React, { useEffect, useState } from "react";
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

// Sample currencies based on Treasury Reporting Rates of Exchange data
const CURRENCIES = [
    { code: "AFN", name: "Afghanistan-Afghani", rate: 70.13 },
    { code: "ALL", name: "Albania-Lek", rate: 83.22 },
    { code: "DZD", name: "Algeria-Dinar", rate: 129.335 },
    { code: "AOA", name: "Angola-Kwanza", rate: 911.955 },
    { code: "XCD", name: "Antigua & Barbuda-East Caribbean Dollar", rate: 2.7 },
    { code: "ARS", name: "Argentina-Peso", rate: 1205.0 },
    { code: "AMD", name: "Armenia-Dram", rate: 390.0 },
    { code: "AUD", name: "Australia-Dollar", rate: 1.531 },
    { code: "AZN", name: "Azerbaijan-Manat", rate: 1.7 },
    { code: "BSD", name: "Bahamas-Dollar", rate: 1.0 },
    { code: "BHD", name: "Bahrain-Dinar", rate: 0.377 },
    { code: "BDT", name: "Bangladesh-Taka", rate: 123.0 },
    { code: "BBD", name: "Barbados-Dollar", rate: 2.02 },
    { code: "BZD", name: "Belize-Dollar", rate: 2.0 },
    { code: "XOF", name: "Benin-Cfa Franc", rate: 556.5 },
    { code: "BMD", name: "Bermuda-Dollar", rate: 1.0 },
    { code: "BOB", name: "Bolivia-Boliviano", rate: 6.85 },
    { code: "BAM", name: "Bosnia-Marka", rate: 1.668 },
    { code: "BWP", name: "Botswana-Pula", rate: 13.245 },
    { code: "BRL", name: "Brazil-Real", rate: 5.478 },
    { code: "BND", name: "Brunei-Dollar", rate: 1.274 },
    { code: "BGN", name: "Bulgaria-Lev New", rate: 1.668 },
    { code: "XOF", name: "Burkina Faso-Cfa Franc", rate: 556.5 },
    { code: "BIF", name: "Burundi-Franc", rate: 2900.0 },
    { code: "KHR", name: "Cambodia-Riel", rate: 3998.5 },
    { code: "XAF", name: "Cameroon-Cfa Franc", rate: 559.36 },
    { code: "CAD", name: "Canada-Dollar", rate: 1.367 },
    { code: "CVE", name: "Cape Verde-Escudo", rate: 94.03 },
    { code: "KYD", name: "Cayman Islands-Dollar", rate: 0.82 },
    { code: "XAF", name: "Central African Republic-Cfa Franc", rate: 559.36 },
    { code: "XAF", name: "Chad-Cfa Franc", rate: 559.36 },
    { code: "CLP", name: "Chile-Peso", rate: 892.19 },
    { code: "CNY", name: "China-Yuan Renminbi", rate: 7.125 },
    { code: "COP", name: "Colombia-Peso", rate: 4158.0 },
    { code: "KMF", name: "Comoros-Franc", rate: 419.0 },
    { code: "CRC", name: "Costa Rica-Colon", rate: 528.76 },
    { code: "HRK", name: "Croatia-Kuna", rate: 6.43 },
    { code: "CUC", name: "Cuba-Peso Convertible", rate: 1.0 },
    { code: "CZK", name: "Czech Republic-Koruna", rate: 21.16 },
    { code: "DKK", name: "Denmark-Krone", rate: 6.362 },
    { code: "DJF", name: "Djibouti-Franc", rate: 177.0 },
    { code: "DOP", name: "Dominican Republic-Peso", rate: 59.11 },
    { code: "USD", name: "Ecuador-Dolares", rate: 1.0 },
    { code: "EGP", name: "Egypt-Pound", rate: 49.49 },
    { code: "EUR", name: "Euro Zone-Euro", rate: 0.853 },
    { code: "FJD", name: "Fiji-Dollar", rate: 2.213 },
    { code: "XAF", name: "Gabon-Cfa Franc", rate: 559.36 },
    { code: "GMD", name: "Gambia-Dalasi", rate: 72.0 },
    { code: "GEL", name: "Georgia-Lari", rate: 2.692 },
    { code: "GHS", name: "Ghana-Cedi", rate: 10.3 },
    { code: "GTQ", name: "Guatemala-Quetzal", rate: 7.68 },
    { code: "GNF", name: "Guinea-Franc", rate: 8631.0 },
    { code: "GYD", name: "Guyana-Dollar", rate: 209.0 },
    { code: "HTG", name: "Haiti-Gourde", rate: 132.0 },
    { code: "HNL", name: "Honduras-Lempira", rate: 24.74 },
    { code: "HUF", name: "Hungary-Forint", rate: 344.01 },
    { code: "ISK", name: "Iceland-Krona", rate: 137.8 },
    { code: "INR", name: "India-Rupee", rate: 83.17 },
    { code: "IDR", name: "Indonesia-Rupiah", rate: 15810.0 },
    { code: "IRR", name: "Iran-Rial", rate: 42000.0 },
    { code: "IQD", name: "Iraq-Dinar", rate: 1309.0 },
    { code: "ILS", name: "Israel-Shekel", rate: 3.369 },
    { code: "JMD", name: "Jamaica-Dollar", rate: 159.0 },
    { code: "JPY", name: "Japan-Yen", rate: 155.2 },
    { code: "JOD", name: "Jordan-Dinar", rate: 0.709 },
    { code: "KZT", name: "Kazakhstan-Tenge", rate: 461.0 },
    { code: "KES", name: "Kenya-Shilling", rate: 152.0 },
    { code: "KWD", name: "Kuwait-Dinar", rate: 0.306 },
    { code: "KGS", name: "Kyrgyzstan-Som", rate: 85.4 },
    { code: "LAK", name: "Laos-Kip", rate: 21000.0 },
    { code: "LBP", name: "Lebanon-Pound", rate: 15000.0 },
    { code: "LSL", name: "Lesotho-Loti", rate: 17.757 },
    { code: "LRD", name: "Liberia-Dollar", rate: 191.0 },
    { code: "LYD", name: "Libya-Dinar", rate: 4.79 },
    { code: "MOP", name: "Macau-Pataca", rate: 8.06 },
    { code: "MKD", name: "Republic Of North Macedonia-Denar", rate: 52.25 },
    { code: "MGA", name: "Madagascar-Ariary", rate: 4540.0 },
    { code: "MWK", name: "Malawi-Kwacha", rate: 1785.0 },
    { code: "MYR", name: "Malaysia-Ringgit", rate: 4.41 },
    { code: "MVR", name: "Maldives-Rufiyaa", rate: 15.42 },
    { code: "XOF", name: "Mali-Cfa Franc", rate: 556.5 },
    { code: "MRU", name: "Mauritania-Ouguiya", rate: 39.9 },
    { code: "MUR", name: "Mauritius-Rupee", rate: 46.32 },
    { code: "MXN", name: "Mexico-Peso", rate: 17.79 },
    { code: "MDL", name: "Moldova-Leu", rate: 17.81 },
    { code: "MNT", name: "Mongolia-Tugrik", rate: 3408.0 },
    { code: "MAD", name: "Morocco-Dirham", rate: 9.62 },
    { code: "MZN", name: "Mozambique-New Metical", rate: 63.86 },
    { code: "MMK", name: "Myanmar-Kyat", rate: 2098.0 },
    { code: "NAD", name: "Namibia-Dollar", rate: 17.757 },
    { code: "NPR", name: "Nepal-Rupee", rate: 133.0 },
    { code: "ANG", name: "Netherlands Antilles-Guilder", rate: 1.79 },
    { code: "NZD", name: "New Zealand-Dollar", rate: 1.639 },
    { code: "NIO", name: "Nicaragua-Cordoba Oro", rate: 36.78 },
    { code: "XOF", name: "Niger-Cfa Franc", rate: 556.5 },
    { code: "NGN", name: "Nigeria-Naira", rate: 1535.0 },
    { code: "NOK", name: "Norway-Krone", rate: 10.535 },
    { code: "OMR", name: "Oman-Rial", rate: 0.385 },
    { code: "PKR", name: "Pakistan-Rupee", rate: 278.7 },
    { code: "PAB", name: "Panama-Balboa", rate: 1.0 },
    { code: "PGK", name: "Papua New Guinea-Kina", rate: 3.95 },
    { code: "PYG", name: "Paraguay-Guarani", rate: 7350.0 },
    { code: "PEN", name: "Peru-Sol", rate: 3.549 },
    { code: "PHP", name: "Philippines-Peso", rate: 56.443 },
    { code: "PLN", name: "Poland-Zloty", rate: 3.617 },
    { code: "QAR", name: "Qatar-Riyal", rate: 3.641 },
    { code: "RON", name: "Romania-New Leu", rate: 4.327 },
    { code: "RUB", name: "Russia-Ruble", rate: 78.4 },
    { code: "RWF", name: "Rwanda-Franc", rate: 1435.0 },
    { code: "STN", name: "Sao Tome & Principe-New Dobras", rate: 20.892 },
    { code: "SAR", name: "Saudi Arabia-Riyal", rate: 3.75 },
    { code: "XOF", name: "Senegal-Cfa Franc", rate: 556.5 },
    { code: "RSD", name: "Serbia-Dinar", rate: 99.77 },
    { code: "SCR", name: "Seychelles-Rupee", rate: 14.16 },
    { code: "SLE", name: "Sierra Leone-New Leone", rate: 22700.0 },
    { code: "SGD", name: "Singapore-Dollar", rate: 1.274 },
    { code: "SBD", name: "Solomon Islands-Dollar", rate: 8.49 },
    { code: "SOS", name: "Somali-Shilling", rate: 568.0 },
    { code: "ZAR", name: "South Africa-Rand", rate: 17.757 },
    { code: "SSP", name: "South Sudan-Sudanese Pound", rate: 4600.0 },
    { code: "LKR", name: "Sri Lanka-Rupee", rate: 299.95 },
    { code: "SRD", name: "Suriname-Dollar", rate: 35.26 },
    { code: "SZL", name: "Eswatini-Lilangeni", rate: 17.757 },
    { code: "SEK", name: "Sweden-Krona", rate: 10.315 },
    { code: "CHF", name: "Switzerland-Franc", rate: 0.842 },
    { code: "SYP", name: "Syria-Pound", rate: 13000.0 },
    { code: "TWD", name: "Taiwan-New Dollar", rate: 31.73 },
    { code: "TJS", name: "Tajikistan-Somoni", rate: 10.6 },
    { code: "TZS", name: "Tanzania-Shilling", rate: 2568.5 },
    { code: "THB", name: "Thailand-Baht", rate: 34.2 },
    { code: "XOF", name: "Togo-Cfa Franc", rate: 556.5 },
    { code: "TOP", name: "Tonga-Pa'anga", rate: 2.306 },
    { code: "TTD", name: "Trinidad & Tobago-Dollar", rate: 6.78 },
    { code: "TND", name: "Tunisia-Dinar", rate: 3.08 },
    { code: "TRY", name: "Turkey-Lira", rate: 34.33 },
    { code: "TMT", name: "Turkmenistan-New Manat", rate: 3.51 },
    { code: "UGX", name: "Uganda-Shilling", rate: 3652.0 },
    { code: "UAH", name: "Ukraine-Hryvnia", rate: 40.97 },
    { code: "AED", name: "United Arab Emirates-Dirham", rate: 3.672 },
    { code: "GBP", name: "United Kingdom-Pound", rate: 0.73 },
    { code: "UYU", name: "Uruguay-Peso", rate: 40.19 },
    { code: "UZS", name: "Uzbekistan-Som", rate: 12631.5 },
    { code: "VUV", name: "Vanuatu-Vatu", rate: 117.0 },
    { code: "VED", name: "Venezuela-Bolivar Soberano", rate: 3600000.0 },
    { code: "VND", name: "Vietnam-Dong", rate: 24680.0 },
    { code: "YER", name: "Yemen-Rial", rate: 250.0 },
    { code: "ZMW", name: "Zambia-Kwacha", rate: 27.2 },
    { code: "ZWL", name: "Zimbabwe-Dollar", rate: 322.0 }
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
    const triggerRef = React.useRef<HTMLButtonElement>(null);

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

                <div className="max-w-4xl mx-auto">
                    {/* Main Transaction Details */}
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

                            {/* Currency Conversion */}
                            <div className="space-y-4">
                                <div className="flex items-center gap-2">
                                    <ArrowLeftRight className="h-5 w-5 text-muted-foreground" />
                                    <h4 className="font-medium text-sm text-muted-foreground">Currency Conversion</h4>
                                </div>
                                <div>
                                    <p className="text-sm text-muted-foreground mb-2">Convert to:</p>
                                    <Popover open={open} onOpenChange={setOpen}>
                                        <PopoverTrigger asChild>
                                            <Button
                                                ref={triggerRef}
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
                                        <PopoverContent
                                            className="p-0"
                                            align="start"
                                            style={{
                                                width: triggerRef.current?.offsetWidth || 'auto'
                                            }}
                                        >
                                            <Command>
                                                <CommandInput placeholder="Search currency..." />
                                                <CommandList className="max-h-[200px]">
                                                    <CommandEmpty>No currency found.</CommandEmpty>
                                                    <CommandGroup>
                                                        {CURRENCIES.map((currency) => (
                                                            <CommandItem
                                                                key={`${currency.code}-${currency.name.replace(/\s/g, '')}`}
                                                                value={`${currency.code} ${currency.name}`}
                                                                onSelect={() => {
                                                                    setSelectedCurrency(currency.code === selectedCurrency ? "" : currency.code);
                                                                    setOpen(false);
                                                                }}
                                                            >
                                                                <Check
                                                                    className={`mr-2 h-4 w-4 ${selectedCurrency === currency.code ? "opacity-100" : "opacity-0"
                                                                        }`}
                                                                />
                                                                <div className="flex items-center justify-between w-full">
                                                                    <span className="flex-1 text-sm">
                                                                        {currency.name}
                                                                    </span>
                                                                    <Badge variant="secondary" className="ml-2 text-xs">
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
                                                    {((transaction.purchaseAmount * (CURRENCIES.find(c => c.code === selectedCurrency)?.rate || 1))).toFixed(2)}
                                                </div>
                                                <div className="text-xs text-muted-foreground">
                                                    {CURRENCIES.find(c => c.code === selectedCurrency)?.code}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                )}
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
            </div>
        </div>
    );
}
