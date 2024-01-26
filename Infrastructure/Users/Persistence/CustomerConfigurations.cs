using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users.Persistence;

public class CustomerConfigurations: IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(c => c.Address,
            ownBuilder =>
            {
                ownBuilder.Property(p => p.City)
                    .HasColumnName(nameof(Address.City))
                    .HasColumnType("nvarchar(50)");

                ownBuilder.Property(p => p.State)
                    .HasColumnName(nameof(Address.State))
                    .HasColumnType("nvarchar(50)");

                ownBuilder.Property(p => p.Street)
                    .HasColumnName(nameof(Address.Street))
                    .HasColumnType("nvarchar(50)");
            });
    }
}
