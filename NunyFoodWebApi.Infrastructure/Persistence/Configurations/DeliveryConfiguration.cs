using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.ReceiverName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(d => d.SignatureUrl)
            .HasMaxLength(500);

        builder.HasIndex(d => d.OrderId)
            .IsUnique();

        builder.HasOne(d => d.Order)
            .WithOne(o => o.Delivery)
            .HasForeignKey<Delivery>(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.DeliveryAgent)
            .WithMany(a => a.Deliveries)
            .HasForeignKey(d => d.DeliveryAgentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
