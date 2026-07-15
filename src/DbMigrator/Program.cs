using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PAS.Assets.Infrastructure;
using PAS.Policies.Infrastructure;

try {
    Console.WriteLine("Initializing database migration...");
    var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Production";
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    var cnc = configuration.GetConnectionString("Db")
        ?? throw new InvalidOperationException("Undefined database connection string.");

    Console.WriteLine("Applying database migration for Asset schema...");
    var assetDbOptionsBuilder = new DbContextOptionsBuilder<AssetDbContext>();
    assetDbOptionsBuilder.UseSqlServer(cnc);
    using (var assetDb = new AssetDbContext(assetDbOptionsBuilder.Options)) {
        await assetDb.Database.MigrateAsync();
        Console.WriteLine("Asset schema migrated.");
    }

    Console.WriteLine("Applying database migration for Policy schema...");
    var policyDbOptionsBuilder = new DbContextOptionsBuilder<PolicyDbContext>();
    policyDbOptionsBuilder.UseSqlServer(cnc);
    using (var policyDb = new PolicyDbContext(policyDbOptionsBuilder.Options)) {
        await policyDb.Database.MigrateAsync();
        Console.WriteLine("Policy schema migrated.");
    }

} catch (Exception ex) {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Database migration critical error: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}
