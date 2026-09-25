using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence.Configurations;

public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(256);
        builder.Property(o => o.CodeHash).IsRequired().HasMaxLength(64);
        builder.Property(o => o.Role).IsRequired().HasMaxLength(50);
        builder.HasIndex(o => new { o.Email, o.Role, o.Purpose });
    }
}
