using Domain.Brands;
using Domain.Categories;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface ICartDbContext
{
     DbSet<Admin> Admins { get; set; }

     DbSet<Customer> Customers { get; set; }
     DbSet<Category>  Categories { get; set; }
      DbSet<Brand> Brands { get; set; }
     DbSet<User> Users { get; set; }
     Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
