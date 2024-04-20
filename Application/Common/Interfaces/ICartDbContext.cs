using Domain.Brands;
using Domain.Categories;
using Domain.Orders;
using Domain.Payments;
using Domain.Products;
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
     DbSet<Order> Orders { get; set; }
     DbSet<Product> Products { get; set; }
    DbSet<Payment> Payments { get; set; }

     Task DispatchEvents(CancellationToken cancellationToken = default);
     Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
