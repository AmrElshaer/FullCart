using Application.Common.Interfaces;
using Domain.Roles;
using Domain.Users;
using Infrastructure.Users.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FullCart.API.UserAccessModule.Infrastructure;


public abstract class ApplicationIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<
    TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>,
    IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    protected ApplicationIdentityDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
public class UserAccessDbContext: ApplicationIdentityDbContext<User, Role, Guid>,IUserAccessDbContext
{
    public UserAccessDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Admin> Admins { get; set; } = default!;

    public DbSet<Customer> Customers { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AdminConfigurations).Assembly);

        base.OnModelCreating(builder);
    }
}