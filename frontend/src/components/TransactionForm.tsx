import { useState } from "react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";

interface Transaction {
  id: string;
  description: string;
  date: string;
  amount: number;
}

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
    id: ""
  });
  
  const [errors, setErrors] = useState<ValidationErrors>({
    description: "",
    date: "",
    amount: "",
    id: ""
  });

  const validateForm = (): boolean => {
    const newErrors: ValidationErrors = {
      description: "",
      date: "",
      amount: "",
      id: ""
    };

    // Description validation
    if (formData.description.length > 50) {
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

    // ID validation
    if (existingIds.includes(formData.id)) {
      newErrors.id = "Id must be unique.";
    }

    setErrors(newErrors);
    return !Object.values(newErrors).some(error => error !== "");
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      const amount = Math.round(parseFloat(formData.amount) * 100) / 100; // Round to nearest cent
      
      onAddTransaction({
        id: formData.id,
        description: formData.description,
        date: formData.date,
        amount: amount
      });

      // Reset form
      setFormData({
        description: "",
        date: "",
        amount: "",
        id: ""
      });
      setErrors({
        description: "",
        date: "",
        amount: "",
        id: ""
      });
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
              <Label htmlFor="id">ID</Label>
              <Input
                id="id"
                value={formData.id}
                onChange={(e) => handleInputChange("id", e.target.value)}
                placeholder="Enter unique transaction ID"
              />
              {errors.id && (
                <p className="text-destructive text-sm">{errors.id}</p>
              )}
            </div>
          </div>
          
          <Button type="submit" className="w-full md:w-auto">
            Create Transaction
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}