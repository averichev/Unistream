using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Configurations;

public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(item => item.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(item => item.CreatedAt)
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.HasIndex(item => item.Name).IsUnique();
    }
}
