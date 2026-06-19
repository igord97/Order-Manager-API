using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

namespace OrderManager.DataLayer.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2(3)");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2(3)");
    }
}