using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Infrastructure.Persistence.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.FullName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(a => a.Email).IsUnique();
        builder.Property(a => a.PasswordHash).IsRequired();
    }
}
