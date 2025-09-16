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
import { ApiService, CurrencyConversionResponse, Transaction } from "../services/api";
import { formatDateTime, formatTransactionDateDetailed } from "../utils/dateUtils";

// Updated currencies based on Treasury Reporting Rates of Exchange data
const CURRENCIES = [
    { code: "AFN", name: "Afghanistan-Afghani" },
    { code: "ALL", name: "Albania-Lek" },
    { code: "DZD", name: "Algeria-Dinar" },
    { code: "AOA", name: "Angola-Kwanza" },
    { code: "XCD", name: "Antigua & Barbuda-East Caribbean Dollar" },
    { code: "ARS", name: "Argentina-Peso" },
    { code: "AMD", name: "Armenia-Dram" },
    { code: "AUD", name: "Australia-Dollar" },
    { code: "EUR", name: "Austria-Euro" },
    { code: "AZN", name: "Azerbaijan-Manat" },
    { code: "AZM", name: "Azerbaijan-New Manat" },
    { code: "BSD", name: "Bahamas-Dollar" },
    { code: "BHD", name: "Bahrain-Dinar" },
    { code: "BDT", name: "Bangladesh-Taka" },
    { code: "BBD", name: "Barbados-Dollar" },
    { code: "BYN", name: "Belarus-New Ruble" },
    { code: "BYR", name: "Belarus-Ruble" },
    { code: "EUR", name: "Belgium-Euro" },
    { code: "BZD", name: "Belize-Dollar" },
    { code: "XOF", name: "Benin-Cfa Franc" },
    { code: "BMD", name: "Bermuda-Dollar" },
    { code: "BOB", name: "Bolivia-Boliviano" },
    { code: "BAM", name: "Bosnia Hercegovina-Marka" },
    { code: "BAM", name: "Bosnia-Marka" },
    { code: "BWP", name: "Botswana-Pula" },
    { code: "BRL", name: "Brazil-Real" },
    { code: "BND", name: "Brunei-Dollar" },
    { code: "BGN", name: "Bulgaria-Lev" },
    { code: "BGN", name: "Bulgaria-Lev New" },
    { code: "XOF", name: "Burkina Faso-Cfa Franc" },
    { code: "MMK", name: "Burma Myanmar-Kyat" },
    { code: "MMK", name: "Burma-Kyat" },
    { code: "BIF", name: "Burundi-Franc" },
    { code: "KHR", name: "Cambodia (Khmer)-Riel" },
    { code: "KHR", name: "Cambodia-Riel" },
    { code: "XAF", name: "Cameroon-Cfa Franc" },
    { code: "CAD", name: "Canada-Dollar" },
    { code: "CVE", name: "Cape Verde-Escudo" },
    { code: "KYD", name: "Cayman Islands-Dollar" },
    { code: "XAF", name: "Central African Republic-Cfa Franc" },
    { code: "XAF", name: "Chad-Cfa Franc" },
    { code: "CLP", name: "Chile-Peso" },
    { code: "CNY", name: "China-Renminbi" },
    { code: "COP", name: "Colombia-Peso" },
    { code: "KMF", name: "Comoros-Franc" },
    { code: "XAF", name: "Congo-Cfa Franc" },
    { code: "CRC", name: "Costa Rica-Colon" },
    { code: "XOF", name: "Cote D'Ivoire-Cfa Franc" },
    { code: "EUR", name: "Croatia-Euro" },
    { code: "HRK", name: "Croatia-Kuna" },
    { code: "EUR", name: "Cross Border-Euro" },
    { code: "CUC", name: "Cuba-Chavito" },
    { code: "CUP", name: "Cuba-Peso" },
    { code: "ANG", name: "Curacao-Caribbean Guilder" },
    { code: "EUR", name: "Cyprus-Euro" },
    { code: "CZK", name: "Czech Republic-Koruna" },
    { code: "CDF", name: "Democratic Republic Of Congo-Congolese Franc" },
    { code: "CDF", name: "Democratic Republic Of Congo-Franc" },
    { code: "DKK", name: "Denmark-Krone" },
    { code: "DJF", name: "Djibouti-Franc" },
    { code: "DOP", name: "Dominican Republic-Peso" },
    { code: "USD", name: "Ecuador-Dolares" },
    { code: "EGP", name: "Egypt-Pound" },
    { code: "USD", name: "El Salvador-Dolares" },
    { code: "USD", name: "El Salvador-Dollar" },
    { code: "XAF", name: "Equatorial Guinea-Cfa Franc" },
    { code: "ERN", name: "Eritrea-Nakfa" },
    { code: "ERN", name: "Eritrea-Nakfa Salary Payment" },
    { code: "EUR", name: "Estonia-Euro" },
    { code: "SZL", name: "Eswatini-Lilangeni" },
    { code: "ETB", name: "Ethiopia-Birr" },
    { code: "EUR", name: "Euro Zone-Euro" },
    { code: "FJD", name: "Fiji-Dollar" },
    { code: "EUR", name: "Finland-Euro" },
    { code: "EUR", name: "France-Euro" },
    { code: "XAF", name: "Gabon-Cfa Franc" },
    { code: "GMD", name: "Gambia-Dalasi" },
    { code: "GEL", name: "Georgia-Lari" },
    { code: "EUR", name: "Germany Frg-Euro" },
    { code: "EUR", name: "Germany-Euro" },
    { code: "GHS", name: "Ghana-Cedi" },
    { code: "EUR", name: "Greece-Euro" },
    { code: "XCD", name: "Grenada-East Caribbean Dollar" },
    { code: "GTQ", name: "Guatemala-Quetzal" },
    { code: "XOF", name: "Guinea Bissau-Cfa Franc" },
    { code: "GNF", name: "Guinea-Franc" },
    { code: "GYD", name: "Guyana-Dollar" },
    { code: "HTG", name: "Haiti-Gourde" },
    { code: "HNL", name: "Honduras-Lempira" },
    { code: "HKD", name: "Hong Kong-Dollar" },
    { code: "HUF", name: "Hungary-Forint" },
    { code: "ISK", name: "Iceland-Krona" },
    { code: "INR", name: "India-Rupee" },
    { code: "IDR", name: "Indonesia-Rupiah" },
    { code: "IRR", name: "Iran-Rial" },
    { code: "IQD", name: "Iraq-Dinar" },
    { code: "EUR", name: "Ireland-Euro" },
    { code: "ILS", name: "Israel-Shekel" },
    { code: "EUR", name: "Italy-Euro" },
    { code: "JMD", name: "Jamaica-Dollar" },
    { code: "JPY", name: "Japan-Yen" },
    { code: "ILS", name: "Jerusalem-Shekel" },
    { code: "JOD", name: "Jordan-Dinar" },
    { code: "KZT", name: "Kazakhstan-Tenge" },
    { code: "KES", name: "Kenya-Shilling" },
    { code: "KRW", name: "Korea-Won" },
    { code: "EUR", name: "Kosovo-Euro" },
    { code: "KWD", name: "Kuwait-Dinar" },
    { code: "KGS", name: "Kyrgyzstan-Som" },
    { code: "LAK", name: "Laos-Kip" },
    { code: "EUR", name: "Latvia-Euro" },
    { code: "LBP", name: "Lebanon-Pound" },
    { code: "LSL", name: "Lesotho-Maloti" },
    { code: "ZAR", name: "Lesotho-South African Rand" },
    { code: "LRD", name: "Liberia-Dollar" },
    { code: "USD", name: "Liberia-U.S. Dollar" },
    { code: "LYD", name: "Libya-Dinar" },
    { code: "EUR", name: "Lithuania-Euro" },
    { code: "EUR", name: "Luxembourg-Euro" },
    { code: "MOP", name: "Macao-Mop" },
    { code: "MKD", name: "Macedonia Fyrom-Denar (Ariary)" },
    { code: "MGA", name: "Madagascar-Ariary" },
    { code: "MGA", name: "Madagascar-Franc" },
    { code: "MWK", name: "Malawi-Kwacha" },
    { code: "MYR", name: "Malaysia-Ringgit" },
    { code: "MVR", name: "Maldives-Rufiyaa" },
    { code: "XOF", name: "Mali-Cfa Franc" },
    { code: "EUR", name: "Malta-Euro" },
    { code: "USD", name: "Marshall Islands-Dollar" },
    { code: "USD", name: "Marshall Islands-U.S. Dollar" },
    { code: "EUR", name: "Martinique-Euro" },
    { code: "MRU", name: "Mauritania-Ouguiya" },
    { code: "MUR", name: "Mauritius-Rupee" },
    { code: "MXN", name: "Mexico-New Peso" },
    { code: "MXN", name: "Mexico-Peso" },
    { code: "USD", name: "Micronesia-Dollar" },
    { code: "USD", name: "Micronesia-U.S. Dollar" },
    { code: "MDL", name: "Moldova-Leu" },
    { code: "MNT", name: "Mongolia-Tugrik" },
    { code: "EUR", name: "Montenegro-Euro" },
    { code: "MAD", name: "Morocco-Dirham" },
    { code: "MZN", name: "Mozambique-Metical" },
    { code: "MMK", name: "Myanmar-Kyat" },
    { code: "NAD", name: "Namibia-Dollar" },
    { code: "NPR", name: "Nepal-Rupee" },
    { code: "ANG", name: "Netherlands Antilles-Guilder" },
    { code: "EUR", name: "Netherlands-Euro" },
    { code: "NZD", name: "New Zealand-Dollar" },
    { code: "NIO", name: "Nicaragua-Cordoba" },
    { code: "XOF", name: "Niger-Cfa Franc" },
    { code: "NGN", name: "Nigeria-Naira" },
    { code: "NOK", name: "Norway-Krone" },
    { code: "OMR", name: "Oman-Rial" },
    { code: "PKR", name: "Pakistan-Rupee" },
    { code: "USD", name: "Palau-Dollar" },
    { code: "PAB", name: "Panama-Balboa" },
    { code: "USD", name: "Panama-Dolares" },
    { code: "PGK", name: "Papua New Guinea-Kina" },
    { code: "PYG", name: "Paraguay-Guarani" },
    { code: "PEN", name: "Peru-Nuevo Sol" },
    { code: "PEN", name: "Peru-Sol" },
    { code: "PHP", name: "Philippines-Peso" },
    { code: "PLN", name: "Poland-Zloty" },
    { code: "EUR", name: "Portugal-Euro" },
    { code: "QAR", name: "Qatar-Riyal" },
    { code: "MKD", name: "Republic Of North Macedonia-Denar" },
    { code: "USD", name: "Republic Of Palau-Dollar" },
    { code: "RON", name: "Romania-Leu" },
    { code: "RON", name: "Romania-New Leu" },
    { code: "RUB", name: "Russia-Ruble" },
    { code: "RWF", name: "Rwanda-Franc" },
    { code: "STD", name: "Sao Tome & Principe-Dobras" },
    { code: "STN", name: "Sao Tome & Principe-New Dobras" },
    { code: "SAR", name: "Saudi Arabia-Riyal" },
    { code: "XOF", name: "Senegal-Cfa Franc" },
    { code: "RSD", name: "Serbia-Dinar" },
    { code: "EUR", name: "Serbia-Euro Dinar" },
    { code: "SCR", name: "Seychelles-Rupee" },
    { code: "SLL", name: "Sierra Leone-Leone" },
    { code: "SLE", name: "Sierra Leone-Old Leone" },
    { code: "SGD", name: "Singapore-Dollar" },
    { code: "EUR", name: "Slovak Republic-Euro" },
    { code: "EUR", name: "Slovakia-Euro" },
    { code: "EUR", name: "Slovenia-Euro" },
    { code: "SBD", name: "Solomon Islands-Dollar" },
    { code: "SOS", name: "Somali-Shilling" },
    { code: "ZAR", name: "South Africa-Rand" },
    { code: "SSP", name: "South Sudan-Sudanese Pound" },
    { code: "EUR", name: "Spain-Euro" },
    { code: "LKR", name: "Sri Lanka-Rupee" },
    { code: "XCD", name: "St. Lucia-East Caribbean Dollar" },
    { code: "SDG", name: "Sudan-Pound" },
    { code: "SDG", name: "Sudan-Sudanese Pound" },
    { code: "SRD", name: "Suriname-Dollar" },
    { code: "SRG", name: "Suriname-Guilder" },
    { code: "SZL", name: "Swaziland-Lilangeni" },
    { code: "SEK", name: "Sweden-Krona" },
    { code: "CHF", name: "Switzerland-Franc" },
    { code: "SYP", name: "Syria-Pound" },
    { code: "TWD", name: "Taiwan-Dollar" },
    { code: "TJS", name: "Tajikistan-Somoni" },
    { code: "TZS", name: "Tanzania-Shilling" },
    { code: "THB", name: "Thailand-Baht" },
    { code: "USD", name: "Timor-Leste Dili" },
    { code: "USD", name: "Timor-Leste-Dili" },
    { code: "XOF", name: "Togo-Cfa Franc" },
    { code: "TOP", name: "Tonga-Pa'Anga" },
    { code: "TTD", name: "Trinidad & Tobago-Dollar" },
    { code: "TND", name: "Tunisia-Dinar" },
    { code: "TRY", name: "Turkey-Lira" },
    { code: "TRY", name: "Turkey-New Lira" },
    { code: "TMT", name: "Turkmenistan-Manat" },
    { code: "TMT", name: "Turkmenistan-New Manat" },
    { code: "UGX", name: "Uganda-Shilling" },
    { code: "UAH", name: "Ukraine-Hryvnia" },
    { code: "AED", name: "United Arab Emirates-Dirham" },
    { code: "GBP", name: "United Kingdom-Pound" },
    { code: "GBP", name: "United Kingdom-Pound Sterling" },
    { code: "UYU", name: "Uruguay-Peso" },
    { code: "UZS", name: "Uzbekistan-Som" },
    { code: "VUV", name: "Vanuatu-Vatu" },
    { code: "VED", name: "Venezuela-Bolivar" },
    { code: "VEF", name: "Venezuela-Bolivar Fuerte" },
    { code: "VED", name: "Venezuela-Bolivar Soberano" },
    { code: "VEF", name: "Venezuela-Fuerte" },
    { code: "VND", name: "Vietnam-Dong" },
    { code: "WST", name: "Western Samoa-Tala" },
    { code: "YER", name: "Yemen-Rial" },
    { code: "ZMW", name: "Zambia-Kwacha" },
    { code: "ZMK", name: "Zambia-New Kwacha" },
    { code: "ZWL", name: "Zimbabwe-Dollar" },
    { code: "XAU", name: "Zimbabwe-Gold" },
    { code: "ZWL", name: "Zimbabwe-Rtgs" },
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
    const [conversionData, setConversionData] = useState<CurrencyConversionResponse | null>(null);
    const [convertingCurrency, setConvertingCurrency] = useState(false);
    const [conversionError, setConversionError] = useState<string | null>(null);
    const triggerRef = React.useRef<HTMLButtonElement>(null);

    // Load transaction details when component mounts
    useEffect(() => {
        if (id) {
            loadTransaction(parseInt(id));
        }
    }, [id]);

    // Convert currency when selectedCurrency changes
    useEffect(() => {
        if (selectedCurrency && transaction) {
            convertCurrency();
        } else {
            setConversionData(null);
            setConversionError(null);
        }
    }, [selectedCurrency, transaction]);

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

    const convertCurrency = async () => {
        if (!transaction || !selectedCurrency) return;

        setConvertingCurrency(true);
        setConversionError(null);

        try {
            // selectedCurrency now stores the currency name directly
            const conversionResult = await ApiService.convertCurrency(
                transaction.transactionDate.split('T')[0], // Convert to YYYY-MM-DD format
                transaction.purchaseAmount,
                selectedCurrency // Use the currency name directly
            );

            setConversionData(conversionResult);
        } catch (err) {
            const errorMessage = err instanceof Error ? err.message : "Failed to convert currency";
            setConversionError(errorMessage);
            setConversionData(null);
        } finally {
            setConvertingCurrency(false);
        }
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
                                                    ? (() => {
                                                        const currency = CURRENCIES.find((currency) => currency.name === selectedCurrency);
                                                        return currency ? `${currency.name} (${currency.code})` : selectedCurrency;
                                                    })()
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
                                                        {CURRENCIES.map((currency, index) => (
                                                            <CommandItem
                                                                key={`${currency.name}-${index}`}
                                                                value={`${currency.code} ${currency.name}`}
                                                                onSelect={() => {
                                                                    setSelectedCurrency(currency.name === selectedCurrency ? "" : currency.name);
                                                                    setOpen(false);
                                                                }}
                                                            >
                                                                <Check
                                                                    className={`mr-2 h-4 w-4 ${selectedCurrency === currency.name ? "opacity-100" : "opacity-0"
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
                                        {convertingCurrency && (
                                            <div className="flex items-center justify-center py-4">
                                                <LoadingSpinner text="Converting currency..." />
                                            </div>
                                        )}

                                        {conversionError && (
                                            <div className="text-center py-4">
                                                <p className="text-red-600 text-sm font-medium">
                                                    {conversionError}
                                                </p>
                                            </div>
                                        )}

                                        {conversionData && !convertingCurrency && !conversionError && (
                                            <>
                                                <div className="flex items-center justify-between text-sm">
                                                    <span className="text-muted-foreground">Exchange Rate (USD → {(() => {
                                                        const currency = CURRENCIES.find(c => c.name === selectedCurrency);
                                                        return currency ? currency.code : selectedCurrency;
                                                    })()}):</span>
                                                    <span className="font-mono">
                                                        {conversionData.exchangeRate.toFixed(4)}
                                                    </span>
                                                </div>
                                                <div className="flex items-center justify-between text-sm">
                                                    <span className="text-muted-foreground">Rate Date:</span>
                                                    <span className="font-mono text-xs">
                                                        {conversionData.exchangeRateDate}
                                                        {!conversionData.isExactDateMatch && (
                                                            <span className="text-yellow-600 ml-1">(approx)</span>
                                                        )}
                                                    </span>
                                                </div>
                                                <Separator />
                                                <div className="flex items-center justify-between">
                                                    <span className="font-medium text-sm text-muted-foreground">Converted Amount:</span>
                                                    <div className="text-right">
                                                        <div className="text-lg font-bold text-green-600">
                                                            {conversionData.convertedAmount.toFixed(2)}
                                                        </div>
                                                        <div className="text-xs text-muted-foreground">
                                                            {(() => {
                                                                const currency = CURRENCIES.find(c => c.name === selectedCurrency);
                                                                return currency ? currency.code : selectedCurrency;
                                                            })()}
                                                        </div>
                                                    </div>
                                                </div>
                                            </>
                                        )}
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
