using Microsoft.EntityFrameworkCore;

namespace Payment.Application.Common;

public interface IPaymentDbContext
{
    DbSet<Domain.Payments.Payment> Payments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}