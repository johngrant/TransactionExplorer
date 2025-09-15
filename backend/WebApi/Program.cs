using Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Data Services
builder.Services.AddDataServices(builder.Configuration);

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
            transactions = "/api/transactions"
        }
    });
})
.WithName("ApiInfo");

app.Run();
