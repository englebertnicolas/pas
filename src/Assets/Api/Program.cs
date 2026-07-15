using Microsoft.EntityFrameworkCore;
using PAS.Assets.Api.Endpoints;
using PAS.Assets.Application;
using PAS.Assets.Application.Funds;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Infrastructure;
using PAS.Common.Api;

var builder = WebApplication.CreateBuilder(args);
var rabbitMqCnc = builder.Configuration.GetConnectionString("RabbitMq") ?? throw new InvalidOperationException("RabbitMq connection string not found.");
var dbCnc = builder.Configuration.GetConnectionString("AssetDb") ?? throw new InvalidOperationException("Asset database connection string not found.");

builder.SetDefaultCulture();
builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddHealthChecks().Services
    .AddDefaultOpenApi()
    .AddDefaultWolverine(dbCnc, AssetDbContext.SchemaName, rabbitMqCnc, builder.Environment.IsDevelopment(), [typeof(IAssetsApplicationMarker).Assembly]);

// Database, repository and query services registration
builder.Services
    .AddDbContext<AssetDbContext>(options => options.UseSqlServer(dbCnc), ServiceLifetime.Scoped, ServiceLifetime.Singleton)
    .AddScoped<IFundRepository, FundRepository>()
    .AddScoped<IFundQueries, FundQueries>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseDefaultOpenApi("PAS.Assets API Reference");
app.UseHttpsRedirection();

// Map the endpoints
app.MapHealthChecks("/health");
app.MapFundEndpoints();

app.Run();


/* INSTALLATION NOTES
 * Install RabbitMQ using Docker: 
 *   docker run -d --name pas -p 5672:5672 -p 15672:15672 rabbitmq:3-management
 */