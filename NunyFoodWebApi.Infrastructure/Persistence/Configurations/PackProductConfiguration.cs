using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence.Configurations;

public class PackProductConfiguration : IEntityTypeConfiguration<PackProduct>
{
    public void Configure(EntityTypeBuilder<PackProduct> builder)
    {
        builder.ToTable("PackProducts");

        builder.HasKey(pp => new { pp.PackId, pp.ProductId });

        builder.Property(pp => pp.Quantity)
            .HasPrecision(18, 2);

        builder.HasOne(pp => pp.Pack)
            .WithMany(p => p.PackProducts)
            .HasForeignKey(pp => pp.PackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Product)
            .WithMany(p => p.PackProducts)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
