using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace PAS.Policies.Infrastructure;

public class PolicyDbContext(DbContextOptions<PolicyDbContext> options) : DbContext(options) {
    public static readonly string SchemaName = "Policy";

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasDefaultSchema(SchemaName);
        builder.ApplyConfigurationsFromAssembly(typeof(PolicyDbContext).Assembly);

        // Est-ce vraiment nécessaire ?
        // (sachant que les tables Wolverine ne sont pas traitées par les migrations EF).
        builder.MapWolverineEnvelopeStorage();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Configure the migrations history table to be in the right schema
        optionsBuilder.UseSqlServer(x =>
            x.MigrationsHistoryTable("__EFMigrationsHistory", SchemaName)
        );
    }
}
