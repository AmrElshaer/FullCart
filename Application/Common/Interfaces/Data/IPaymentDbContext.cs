using Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Data;

public interface IPaymentDbContext
{
    DbSet<Payment> Payments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}