using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface ICartDbContext
{
     DbSet<Admin> Admins { get; set; }

     DbSet<Customer> Customers { get; set; }
}
