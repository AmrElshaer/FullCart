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
using MediatR;
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

public class CartDbContext : ApplicationIdentityDbContext<User, Role, Guid>, ICartDbContext
{
    private readonly IMediator _mediator;

    public CartDbContext(DbContextOptions<CartDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
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
        var domainEventEntities = ChangeTracker.Entries<Entity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        var integrationEvents = new List<INotification>();

        foreach (var entity in domainEventEntities)
        {
            while (entity.DomainEvents.TryTake(out var domainEvent))
            {
                var integrationEvent = CreateDomainEventNotification(domainEvent);

                if (integrationEvent is not null)
                {
                    integrationEvents.Add(integrationEvent);
                }

                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        var tasks = integrationEvents
            .Select(async (domainEvent) =>
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            });

        await Task.WhenAll(tasks);

        return result;
    }

    private static INotification? CreateDomainEventNotification(IDomainEvent domainEvent)
    {
        var genericDispatcherType = typeof(IntegrationEvent<>).MakeGenericType(domainEvent.GetType());

        return (INotification?) Activator.CreateInstance(genericDispatcherType, domainEvent);
    }
}
