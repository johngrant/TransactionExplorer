import { Calendar, Clock, DollarSign, FileText, Hash, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { ApiService, Transaction } from "../services/api";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from "./ui/alert-dialog";
import { Badge } from "./ui/badge";
import { Button } from "./ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "./ui/dialog";
import { LoadingSpinner } from "./ui/loading-spinner";
import { Separator } from "./ui/separator";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "./ui/sheet";
import { useIsMobile } from "./ui/use-mobile";

interface TransactionDetailsPopupProps {
    transactionId: number | null;
    isOpen: boolean;
    onClose: () => void;
    onDelete?: (transactionId: number) => void;
}

export function TransactionDetailsPopupAlternative({
    transactionId,
    isOpen,
    onClose,
    onDelete,
}: TransactionDetailsPopupProps) {
    const [transaction, setTransaction] = useState<Transaction | null>(null);
    const [loading, setLoading] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [showDeleteDialog, setShowDeleteDialog] = useState(false);
    const isMobile = useIsMobile();

    // Load transaction details when popup opens
    useEffect(() => {
        if (isOpen && transactionId) {
            loadTransaction();
        }
    }, [isOpen, transactionId]);

    // Handle ESC key to close
    useEffect(() => {
        const handleEscape = (event: KeyboardEvent) => {
            if (event.key === "Escape" && isOpen) {
                onClose();
            }
        };

        if (isOpen) {
            document.addEventListener("keydown", handleEscape);
            return () => document.removeEventListener("keydown", handleEscape);
        }
    }, [isOpen, onClose]);

    // Clear state when closing
    useEffect(() => {
        if (!isOpen) {
            setTransaction(null);
            setError(null);
            setLoading(false);
            setDeleting(false);
            setShowDeleteDialog(false);
        }
    }, [isOpen]);

    const loadTransaction = async () => {
        if (!transactionId) return;

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
        if (!transactionId) return;

        setDeleting(true);
        try {
            await ApiService.deleteTransaction(transactionId);
            onDelete?.(transactionId);
            onClose();
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to delete transaction");
        } finally {
            setDeleting(false);
            setShowDeleteDialog(false);
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString("en-US", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
        });
    };

    const formatDateTime = (dateString: string) => {
        return new Date(dateString).toLocaleString("en-US", {
            year: "numeric",
            month: "short",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        });
    };

    const renderContent = () => {
        if (loading) {
            return (
                <div className="flex items-center justify-center py-8">
                    <LoadingSpinner text="Loading transaction details..." size="md" />
                </div>
            );
        }

        if (error) {
            return (
                <div className="flex flex-col items-center justify-center py-8 text-center">
                    <p className="text-destructive mb-4">{error}</p>
                    <Button variant="outline" onClick={onClose}>
                        Close
                    </Button>
                </div>
            );
        }

        if (!transaction) {
            return (
                <div className="flex items-center justify-center py-8">
                    <p className="text-muted-foreground">No transaction selected</p>
                </div>
            );
        }

        return (
            <>
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
                        <div className="text-2xl font-bold text-green-600">
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
                            <p className="text-foreground">{formatDate(transaction.transactionDate)}</p>
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
            </>
        );
    };

    // Solution 2: Using DialogFooter Component
    if (isMobile) {
        return (
            <Sheet open={isOpen} onOpenChange={onClose}>
                <SheetContent
                    side="bottom"
                    className="h-full max-h-screen inset-x-0 bottom-0 top-0 data-[state=closed]:slide-out-to-bottom data-[state=open]:slide-in-from-bottom flex flex-col p-6 border-t"
                >
                    <SheetHeader className="text-left mb-6">
                        <SheetTitle>Transaction Details</SheetTitle>
                    </SheetHeader>
                    <div className="flex-1 overflow-y-auto pr-2 pb-4">
                        {renderContent()}
                    </div>
                    {!loading && !error && transaction && (
                        <div className="flex justify-end gap-2 pt-4 border-t bg-background">
                            <Button variant="outline" onClick={onClose}>
                                Close
                            </Button>
                            <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
                                <AlertDialogTrigger asChild>
                                    <Button variant="destructive" className="flex items-center gap-2">
                                        <Trash2 className="h-4 w-4" />
                                        Delete
                                    </Button>
                                </AlertDialogTrigger>
                                <AlertDialogContent>
                                    <AlertDialogHeader>
                                        <AlertDialogTitle>Delete Transaction</AlertDialogTitle>
                                        <AlertDialogDescription>
                                            Are you sure you want to delete transaction #{transaction.id}? This action cannot be undone.
                                        </AlertDialogDescription>
                                    </AlertDialogHeader>
                                    <AlertDialogFooter>
                                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                                        <AlertDialogAction
                                            onClick={handleDelete}
                                            disabled={deleting}
                                            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                                        >
                                            {deleting ? "Deleting..." : "Delete"}
                                        </AlertDialogAction>
                                    </AlertDialogFooter>
                                </AlertDialogContent>
                            </AlertDialog>
                        </div>
                    )}
                </SheetContent>
            </Sheet>
        );
    }

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="max-w-2xl max-h-[90vh] flex flex-col">
                <DialogHeader>
                    <DialogTitle>Transaction Details</DialogTitle>
                </DialogHeader>
                <div className="flex-1 overflow-y-auto pr-2">
                    {renderContent()}
                </div>
                {!loading && !error && transaction && (
                    <DialogFooter className="pt-4 border-t">
                        <Button variant="outline" onClick={onClose}>
                            Close
                        </Button>
                        <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
                            <AlertDialogTrigger asChild>
                                <Button variant="destructive" className="flex items-center gap-2">
                                    <Trash2 className="h-4 w-4" />
                                    Delete
                                </Button>
                            </AlertDialogTrigger>
                            <AlertDialogContent>
                                <AlertDialogHeader>
                                    <AlertDialogTitle>Delete Transaction</AlertDialogTitle>
                                    <AlertDialogDescription>
                                        Are you sure you want to delete transaction #{transaction.id}? This action cannot be undone.
                                    </AlertDialogDescription>
                                </AlertDialogHeader>
                                <AlertDialogFooter>
                                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                                    <AlertDialogAction
                                        onClick={handleDelete}
                                        disabled={deleting}
                                        className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                                    >
                                        {deleting ? "Deleting..." : "Delete"}
                                    </AlertDialogAction>
                                </AlertDialogFooter>
                            </AlertDialogContent>
                        </AlertDialog>
                    </DialogFooter>
                )}
            </DialogContent>
        </Dialog>
    );
}
