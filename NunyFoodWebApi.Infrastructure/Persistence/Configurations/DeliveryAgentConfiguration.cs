using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence.Configurations;

public class DeliveryAgentConfiguration : IEntityTypeConfiguration<DeliveryAgent>
{
    public void Configure(EntityTypeBuilder<DeliveryAgent> builder)
    {
        builder.ToTable("DeliveryAgents");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(a => a.Email).IsUnique();
        builder.Property(a => a.PasswordHash).IsRequired();
        builder.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(a => a.Zone).IsRequired().HasMaxLength(100);
    }
}
