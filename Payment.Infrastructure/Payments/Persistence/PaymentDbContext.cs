using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Common;

namespace Payment.Infrastructure.Payments.Persistence;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options), IPaymentDbContext
{
    public DbSet<Payment.Domain.Payments.Payment> Payments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}