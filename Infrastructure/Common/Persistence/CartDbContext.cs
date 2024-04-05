using System.Reflection;
using Application.Common.Interfaces;
using Domain.Brands;
using Domain.Categories;
using Domain.Common;
using Domain.Orders;
using Domain.Payments;
using Domain.Products;
using Domain.Roles;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Persistence;


public abstract class ApplicationIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<
    TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>,
    IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    protected ApplicationIdentityDbContext(DbContextOptions options)
        : base(options) { }
}

public class CartDbContext : ApplicationIdentityDbContext<User, Role, Guid>,ICartDbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CartDbContext(DbContextOptions<CartDbContext> options,IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        this._domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Admin> Admins { get; set; } = default!;

    public DbSet<Customer> Customers { get; set; } = default!;

    public DbSet<Category> Categories { get; set; } = default!;

    public DbSet<Brand> Brands { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Payment> Payments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _domainEventDispatcher.Dispatch();
        return await base.SaveChangesAsync(cancellationToken);
    }

}

