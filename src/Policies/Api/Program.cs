using Microsoft.EntityFrameworkCore;
using PAS.Common.Api;
using PAS.Common.ServiceDefaults;
using PAS.Policies.Api.Endpoints;
using PAS.Policies.Application;
using PAS.Policies.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var rabbitMqCnc = builder.Configuration.GetConnectionString("RabbitMq") ?? throw new InvalidOperationException("RabbitMq connection string not found.");
var dbCnc = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Database connection string not found.");

builder
    .AddServiceDefaults()
    .SetDefaultCulture();

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddDefaultOpenApi()
    .AddDefaultWolverine(dbCnc, PolicyDbContext.SchemaName, rabbitMqCnc, builder.Environment.IsDevelopment(), [typeof(IPoliciesApplicationMarker).Assembly]);

// Database, repository and query services registration
builder.Services
    .AddDbContext<PolicyDbContext>(options => options.UseSqlServer(dbCnc), ServiceLifetime.Scoped, ServiceLifetime.Singleton)
    //.AddScoped<IPolicyRepository, PolicyRepository>()
    //.AddScoped<IPolicyQueryService, PolicyQueryService>()
    ;

var app = builder.Build();
app.UseExceptionHandler();
app.UseDefaultOpenApi("PAS.Policies API Reference");
app.UseHttpsRedirection();

// Map the endpoints
app.MapDefaultEndpoints();
app.MapPolicyEndpoints();

app.Run();
