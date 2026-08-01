using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.OpenApi.Models;
using OrderManagement.Application.Behaviors;
using OrderManagement.Application.Commands.Orders.AddItem;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;
using OrderManagement.Application.Validators;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Data;
using OrderManagement.Infrastructure.Repositories.Read;
using OrderManagement.Infrastructure.Repositories.Write;
using OrderManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONTROLLERS
// ============================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ============================================================
// 2. SWAGGER / OPENAPI
// ============================================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Management API (CQRS + Dapper)",
        Version = "v1",
        Description = "Order Management System using CQRS, MediatR, Dapper, Stored Procedures, Redis and AutoMapper."
    });
});

// ============================================================
// 3. DAPPER CONTEXT
// ============================================================

builder.Services.AddScoped<IDapperContext, DapperContext>();

// ============================================================
// 4. MEDIATR / CQRS - ✅ FIXED
// ============================================================

// ✅ FIX: Register FluentValidation manually instead of using AddValidatorsFromAssembly
// This avoids the ReflectionTypeLoadException
builder.Services.AddScoped<IValidator<AddItemCommand>, AddItemCommandValidator>();
// Add other validators here if needed

builder.Services.AddMediatR(configuration =>
{
    // Register handlers from Application assembly
    configuration.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly);

    // Logging pipeline behavior
    configuration.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(LoggingBehavior<,>)
    );

    // Validation pipeline behavior
    configuration.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>)
    );
});

// ============================================================
// 5. REPOSITORIES
// ============================================================

// Write repositories
builder.Services.AddScoped<IWriteRepository<Order>, OrderWriteRepository>();
builder.Services.AddScoped<IWriteRepository<Product>, ProductWriteRepository>();

// Read repositories
builder.Services.AddScoped<IReadRepository<Order>, OrderReadRepository>();
builder.Services.AddScoped<IReadRepository<Product>, ProductReadRepository>();

// ============================================================
// 6. APPLICATION SERVICES
// ============================================================

builder.Services.AddScoped<IOrderCalculator, OrderCalculator>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// ============================================================
// 7. AUTOMAPPER - ✅ FIXED (Inline Configuration)
// ============================================================

// This avoids assembly scanning entirely
builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<Order, OrderDto>()
        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
        .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.DiscountType ?? "None"));

    cfg.CreateMap<OrderItem, OrderItemDto>();

    cfg.CreateMap<Product, ProductDto>()
        .ForMember(dest => dest.TaxStatus,
            opt => opt.MapFrom(src => src.IsTaxable ? "Taxable" : "Zero-Rated"));
});

// ============================================================
// 8. CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ============================================================
// 9. REDIS CACHE
// ============================================================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "OrderManagement_";
});

// ============================================================
// 10. HEALTH CHECKS
// ============================================================

builder.Services.AddHealthChecks();

// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();

// ============================================================
// HTTP REQUEST PIPELINE
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();

// ============================================================
// ENDPOINTS
// ============================================================

app.MapControllers();

// Health endpoints
app.MapHealthChecks("/health");

app.MapGet("/health/simple", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));

app.Run();