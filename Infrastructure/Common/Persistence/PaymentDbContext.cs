using System.Reflection;
using Application.Common.Interfaces.Data;
using Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Persistence;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options), IPaymentDbContext
{
    public DbSet<Payment> Payments { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

}