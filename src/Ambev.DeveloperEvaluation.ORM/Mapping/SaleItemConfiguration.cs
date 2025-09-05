using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);
        builder.Property(si => si.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(si => si.ProductId).IsRequired().HasMaxLength(100);
        builder.Property(si => si.ProductName).IsRequired().HasMaxLength(200);
        
        builder.Property(si => si.Quantity).IsRequired();
        builder.Property(si => si.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        
        builder.Property(si => si.DiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Property(si => si.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(si => si.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        
        builder.Property(si => si.IsCancelled).IsRequired().HasDefaultValue(false);

        builder.Property(si => si.CreatedAt).IsRequired();
        builder.Property(si => si.UpdatedAt);

        builder.Property(si => si.SaleId).IsRequired();

        // Relationships
        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId);

        // Ignore domain events for persistence
        builder.Ignore(si => si.DomainEvents);
    }
}
