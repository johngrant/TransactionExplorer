import { AlertCircle, CheckCircle2, Clock } from "lucide-react";

interface StatusIndicatorProps {
    status: "loading" | "success" | "error";
    message?: string;
}

export function StatusIndicator({ status, message }: StatusIndicatorProps) {
    const statusConfig = {
        loading: {
            icon: Clock,
            className: "text-blue-500",
            defaultMessage: "Loading..."
        },
        success: {
            icon: CheckCircle2,
            className: "text-green-500",
            defaultMessage: "Success"
        },
        error: {
            icon: AlertCircle,
            className: "text-red-500",
            defaultMessage: "Error occurred"
        }
    };

    const config = statusConfig[status];
    const Icon = config.icon;

    return (
        <div className={`flex items-center gap-2 text-sm ${config.className}`}>
            <Icon className="h-4 w-4" />
            <span>{message || config.defaultMessage}</span>
        </div>
    );
}
