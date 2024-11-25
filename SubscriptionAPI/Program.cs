using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Infrastructure.Factories;
using SubscriptionAPI.Infrastructure.Middleware;
using SubscriptionAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Subscription API",
        Version = "v1",
        Description = "API for managing SMS-based subscription services"
    });

    c.AddSecurityDefinition("Idempotency-Key", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "Idempotency-Key",
        Description = "Unique key for idempotent requests"
    });

    // Ensure that all requests include the Idempotency-Key header
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Idempotency-Key"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddDbContext<SubscriptionAPI.Infrastructure.Data.AppDbContext>(options =>
    options.UseInMemoryDatabase("SubscriptionDb"));

// Register other services (e.g., IMemoryCache, MediatR, etc.)
builder.Services.AddMemoryCache();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// Add services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISubscriptionFactory, SubscriptionFactory>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddSingleton<IIdempotencyService, IdempotencyService>();

// Configure caching
builder.Services.AddDistributedMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100 MB cache size limit
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Subscription API v1");
        c.RoutePrefix = "swagger";

    });
}

// Add middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SubscriptionAPI.Infrastructure.Data.AppDbContext>();
    DbInitializer.Initialize(dbContext);
}

app.Run();
