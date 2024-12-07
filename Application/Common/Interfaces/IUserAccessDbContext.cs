using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IUserAccessDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; } 

    public DbSet<Customer> Customers { get; set; } 
}