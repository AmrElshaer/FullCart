using Domain.Categories;
using Domain.Orders;
using Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Payments.Persistence
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasOne<Order>()
             .WithMany()
             .HasForeignKey(p=>p.OrderId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
