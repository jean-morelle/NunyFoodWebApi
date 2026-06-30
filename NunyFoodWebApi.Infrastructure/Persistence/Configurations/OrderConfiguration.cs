using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Amount)
            .HasPrecision(18, 2);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Beneficiary)
            .WithMany(b => b.Orders)
            .HasForeignKey(o => o.BeneficiaryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Pack)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PackId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
