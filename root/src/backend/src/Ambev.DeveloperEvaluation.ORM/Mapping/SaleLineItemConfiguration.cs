using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleLineItemConfiguration : IEntityTypeConfiguration<SaleLineItem>
{
    public void Configure(EntityTypeBuilder<SaleLineItem> builder)
    {
        builder.ToTable("SaleLineItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.SaleId).IsRequired();

        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);

        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.DiscountAmount).HasPrecision(18, 2);
        builder.Property(i => i.LineTotal).HasPrecision(18, 2);
        builder.Property(i => i.IsCancelled).IsRequired();
    }
}
