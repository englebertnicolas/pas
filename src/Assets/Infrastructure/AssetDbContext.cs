using Microsoft.EntityFrameworkCore;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Infrastructure.EntityConfigurations;
using Wolverine.EntityFrameworkCore;

namespace PAS.Assets.Infrastructure;

public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContext(options) {
    public static readonly string SchemaName = "Asset";
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Fund> Funds { get; set; }
    public DbSet<FundNav> FundNavs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasDefaultSchema(SchemaName);
        builder.ApplyConfiguration(new CurrencyEntityTypeConfiguration());
        builder.ApplyConfiguration(new FundEntityTypeConfiguration());

        // Est-ce vraiment nécessaire ?
        // (Les tables Wolverine ne sont pas traitées par les migrations EF).
        builder.MapWolverineEnvelopeStorage();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Configure the migrations history table to be in the right schema
        optionsBuilder.UseSqlServer(x =>
            x.MigrationsHistoryTable("__EFMigrationsHistory", SchemaName)
        );
    }
}
