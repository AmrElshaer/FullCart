using Domain.Brands;
using Domain.Categories;
using Domain.Comments;
using Domain.Orders;
using Domain.Products;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;

public interface ICartDbContext
{
    
    DbSet<Category> Categories { get; set; }
    DbSet<Brand> Brands { get; set; }
   
    DbSet<Order> Orders { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Comment> Comments { get; set; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}