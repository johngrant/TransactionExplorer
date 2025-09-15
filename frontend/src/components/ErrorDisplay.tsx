import { AlertCircle, RefreshCw } from "lucide-react";
import { Button } from "./ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";

interface ErrorDisplayProps {
    error: string;
    onRetry?: () => void;
    title?: string;
}

export function ErrorDisplay({ error, onRetry, title = "Something went wrong" }: ErrorDisplayProps) {
    return (
        <Card className="border-destructive/20">
            <CardHeader>
                <CardTitle className="flex items-center gap-2 text-destructive">
                    <AlertCircle className="h-5 w-5" />
                    {title}
                </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
                <p className="text-sm text-muted-foreground">
                    {error}
                </p>
                {onRetry && (
                    <Button
                        onClick={onRetry}
                        variant="outline"
                        size="sm"
                        className="flex items-center gap-2"
                    >
                        <RefreshCw className="h-4 w-4" />
                        Try Again
                    </Button>
                )}
            </CardContent>
        </Card>
    );
}
