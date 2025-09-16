using Data.Extensions;
using Services.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.Converters;
using System.Reflection;

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

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Transaction Explorer API",
        Version = "v1",
        Description = "API for managing transactions and exchange rates",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Transaction Explorer Support",
            Email = "support@example.com"
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction Explorer API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    });
}

// Enable CORS
app.UseCors("AllowAll");

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
        documentation = new
        {
            swagger = "/swagger",
            openapi = "/swagger/v1/swagger.json"
        },
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
