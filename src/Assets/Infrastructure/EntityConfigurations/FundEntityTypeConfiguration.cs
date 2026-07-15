using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Infrastructure.EntityConfigurations;

internal class FundEntityTypeConfiguration : IEntityTypeConfiguration<Fund> {

    public void Configure(EntityTypeBuilder<Fund> fundBuilder) {
        fundBuilder.ToTable("Funds");

        fundBuilder.Ignore(e => e.DomainEvents);
        fundBuilder
            .Property(e => e.Id)
            .UseHiLo("FundSeq");

        fundBuilder
            .Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(128);

        fundBuilder
            .Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(128);

        fundBuilder
            .Property(e => e.Name)
            .HasMaxLength(128);

        fundBuilder.ComplexProperty(e => e.Isin, isinBuilder => {
            isinBuilder
                .Property(e => e.Value)
                .HasColumnName("Isin")
                .HasMaxLength(12);
        });

        fundBuilder.ComplexProperty(e => e.Currency, currencyBuilder => {
            currencyBuilder
                .Property(e => e.Code)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        fundBuilder.OwnsMany(e => e.Navs, navsBuilder => {
            navsBuilder.ToTable("FundNavs");
            navsBuilder
                .Property<int>("Id")
                .UseHiLo("FundNavSeq");

            navsBuilder.HasIndex(e => e.Date).IsUnique();
        });

        // Index creation on ComplexProperty is not yet supported in EF Core: https://github.com/dotnet/efcore/issues/31246
        // Fixed in Net11: https://github.com/dotnet/efcore/pull/38192
        //fundBuilder.HasIndex(e => e.Isin).IsUnique();

        fundBuilder.HasIndex(e => e.Name).IsUnique();
        fundBuilder.HasIndex(e => e.Status);
    }
}
