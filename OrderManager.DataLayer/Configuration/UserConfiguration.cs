using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

namespace OrderManager.DataLayer.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2(3)");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2(3)");

        builder.HasMany(x => x.Orders)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}