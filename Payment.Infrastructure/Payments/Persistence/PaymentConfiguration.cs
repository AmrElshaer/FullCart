using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Infrastructure.Payments.Persistence;

internal class PaymentConfiguration : IEntityTypeConfiguration<Domain.Payments.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Payments.Payment> builder)
    {
        builder.ToTable("Payments");
    }
}