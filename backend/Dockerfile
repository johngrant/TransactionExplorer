# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first for better caching
COPY ["TransactionExplorer.sln", "./"]
COPY ["WebApi/WebApi.csproj", "WebApi/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Tests/Tests.csproj", "Tests/"]

# Restore dependencies (cached if project files don't change)
RUN dotnet restore "WebApi/WebApi.csproj"

# Copy all source code
COPY . ./

# Build and publish in one step for faster builds
RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

# Copy published application
COPY --from=build /app/publish .

# Change ownership of the application files
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 5070

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5070
ENV ASPNETCORE_ENVIRONMENT=Development

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:5070/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "WebApi.dll"]
