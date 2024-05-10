using Domain.Orders;
using Domain.Products;
using Domain.Users;
using EFCore.AuditExtensions.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Orders.Persistence;

public class OrderConfiguration: IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,4)");

        builder.HasOne<Customer>(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsMany(o => o.Items,
            i =>
            {
                
                
                i.WithOwner().HasForeignKey("OrderId");
                i.HasKey("OrderId", "ProductId");
                i.HasOne<Product>(i => i.Product)
                    .WithMany()
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
                i.OwnsOne(c => c.ProductPrice,
                    ownBuilder =>
                    {

                        ownBuilder.Property(p => p.Price)
                            .HasColumnName(nameof(ProductPrice.Price))
                            .HasColumnType("decimal(18,4)");
                    });
                i.OwnsOne(c => c.Quantity,
                    ownBuilder =>
                    {

                        ownBuilder.Property(p => p.Quantity)
                            .HasColumnName(nameof(OrderItemQuantity.Quantity));
                    });
               
                
            });

        builder.IsAudited();
    }
}


