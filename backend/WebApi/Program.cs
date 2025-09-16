using Data.Extensions;
using Services.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure DateOnly serialization to return just the date part
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // Add DateOnly converter to serialize as YYYY-MM-DD instead of datetime
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

// Add Data Services
builder.Services.AddDataServices(builder.Configuration);

// Add Treasury Exchange Rate Client Services
builder.Services.AddTreasuryExchangeRateClient(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
})
.WithName("HealthCheck");

// Default API info endpoint
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        message = "Transaction Explorer API is running",
        service = "WebApi",
        version = "1.0.0",
        timestamp = DateTime.UtcNow,
        endpoints = new
        {
            health = "/health",
            transactions = "/api/transactions",
            exchangeRates = new
            {
                rates = "/api/exchangerate/rates?transactionDate=YYYY-MM-DD&countryCurrencyDesc=Currency-Description",
                latest = "/api/exchangerate/latest?transactionDate=YYYY-MM-DD&countryCurrencyDesc=Currency-Description",
                convert = "/api/exchangerate/convert?transactionDate=YYYY-MM-DD&amountUsd=100.00&countryCurrencyDesc=Currency-Description"
            }
        }
    });
})
.WithName("ApiInfo");

app.Run();
