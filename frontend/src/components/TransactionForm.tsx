import { useState } from "react";
import { ApiService, Transaction } from "../services/api";
import { Button } from "./ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";
import { Input } from "./ui/input";
import { Label } from "./ui/label";

interface TransactionFormProps {
  onAddTransaction: (transaction: Transaction) => void;
  existingIds: string[];
}

interface ValidationErrors {
  description: string;
  date: string;
  amount: string;
  id: string;
}

export function TransactionForm({ onAddTransaction, existingIds }: TransactionFormProps) {
  const [formData, setFormData] = useState({
    description: "",
    date: "",
    amount: "",
    customId: ""
  });

  const [errors, setErrors] = useState<ValidationErrors>({
    description: "",
    date: "",
    amount: "",
    id: ""
  });

  const [isSubmitting, setIsSubmitting] = useState(false);

  const validateForm = (): boolean => {
    const newErrors: ValidationErrors = {
      description: "",
      date: "",
      amount: "",
      id: ""
    };

    // Description validation
    if (!formData.description.trim()) {
      newErrors.description = "Description is required.";
    } else if (formData.description.length > 50) {
      newErrors.description = "Description too long. Must be less than 50 chars.";
    }

    // Date validation
    if (!formData.date || isNaN(Date.parse(formData.date))) {
      newErrors.date = "Transaction Date must be a valid Date format.";
    }

    // Amount validation
    const amount = parseFloat(formData.amount);
    if (isNaN(amount) || amount <= 0) {
      newErrors.amount = "Purchase amount must be valid rounded to nearest cent.";
    }

    // Custom ID validation
    if (existingIds.includes(formData.customId)) {
      newErrors.id = "Id must be unique.";
    }

    if (!formData.customId.trim()) {
      newErrors.id = "Custom ID is required.";
    }

    if (formData.customId.length > 100) {
      newErrors.id = "Custom ID cannot exceed 100 characters.";
    }

    setErrors(newErrors);
    return !Object.values(newErrors).some(error => error !== "");
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      const amount = Math.round(parseFloat(formData.amount) * 100) / 100; // Round to nearest cent

      // Format date as ISO string for the API
      const transactionDate = new Date(formData.date).toISOString();

      const newTransaction = await ApiService.createTransaction({
        customId: formData.customId,
        description: formData.description,
        transactionDate: transactionDate,
        purchaseAmount: amount
      });

      onAddTransaction(newTransaction);

      // Reset form
      setFormData({
        description: "",
        date: "",
        amount: "",
        customId: ""
      });
      setErrors({
        description: "",
        date: "",
        amount: "",
        id: ""
      });
    } catch (error) {
      console.error('Error creating transaction:', error);

      if (error instanceof Error) {
        // Try to parse server validation errors if available
        const errorMessage = error.message;
        if (errorMessage.includes('Description')) {
          setErrors(prev => ({ ...prev, description: 'Description is required' }));
        } else if (errorMessage.includes('CustomId')) {
          setErrors(prev => ({ ...prev, id: 'Custom ID is required' }));
        } else if (errorMessage.includes('PurchaseAmount')) {
          setErrors(prev => ({ ...prev, amount: 'Purchase amount must be greater than 0' }));
        } else if (errorMessage.includes('TransactionDate')) {
          setErrors(prev => ({ ...prev, date: 'Invalid transaction date' }));
        } else {
          setErrors(prev => ({ ...prev, id: errorMessage }));
        }
      } else {
        setErrors(prev => ({ ...prev, id: 'Failed to create transaction' }));
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleInputChange = (field: keyof typeof formData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: "" }));
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Create New Transaction</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Input
                id="description"
                value={formData.description}
                onChange={(e) => handleInputChange("description", e.target.value)}
                placeholder="Enter transaction description"
              />
              {errors.description && (
                <p className="text-destructive text-sm">{errors.description}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="date">Transaction Date</Label>
              <Input
                id="date"
                type="date"
                value={formData.date}
                onChange={(e) => handleInputChange("date", e.target.value)}
              />
              {errors.date && (
                <p className="text-destructive text-sm">{errors.date}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="amount">Purchase Amount</Label>
              <Input
                id="amount"
                type="number"
                step="0.01"
                value={formData.amount}
                onChange={(e) => handleInputChange("amount", e.target.value)}
                placeholder="0.00"
              />
              {errors.amount && (
                <p className="text-destructive text-sm">{errors.amount}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="customId">Custom ID</Label>
              <Input
                id="customId"
                value={formData.customId}
                onChange={(e) => handleInputChange("customId", e.target.value)}
                placeholder="Enter unique transaction ID"
              />
              {errors.id && (
                <p className="text-destructive text-sm">{errors.id}</p>
              )}
            </div>
          </div>

          <Button
            type="submit"
            className="w-full md:w-auto"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Creating..." : "Create Transaction"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
