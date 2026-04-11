using Banking.Api.Middleware;
using Banking.Core.Interfaces;
using Banking.Core.Services;
using Banking.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers to the container
builder.Services.AddControllers();

// Added Swagger/OpenAPI support for quick testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register services
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();