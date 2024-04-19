using System.Data;
using System.Reflection;
using Application.Common.Interfaces;
using Domain.Brands;
using Domain.Categories;
using Domain.Common;
using Domain.Orders;
using Domain.Outbox;
using Domain.Payments;
using Domain.Products;
using Domain.Roles;
using Domain.Users;
using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
    private readonly ICapPublisher _integrationEventPublisher;

    public CartDbContext(DbContextOptions<CartDbContext> options, IMediator mediator, ICapPublisher integrationEventPublisher)
        : base(options)
    {
        _mediator = mediator;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public DbSet<Admin> Admins { get; set; } = default!;

    public DbSet<Customer> Customers { get; set; } = default!;

    public DbSet<Category> Categories { get; set; } = default!;

    public DbSet<Brand> Brands { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<Payment> Payments { get; set; } = default!;

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;

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

        var integrationEventsEntities = ChangeTracker.Entries<Entity>()
            .Select(po => po.Entity)
            .Where(po => po.IntegrationEvents.Any())
            .ToArray();

        // ReSharper disable once PossibleMultipleEnumeration
        if (domainEventEntities.Length == 0 && integrationEventsEntities.ToArray().Length == 0)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        foreach (var entity in domainEventEntities)
        {
            while (entity.DomainEvents.TryTake(out var domainEvent))
            {
                await _mediator.Publish(domainEvent);
            }
        }
       
        foreach (var entity in integrationEventsEntities)
        {
            while (entity.IntegrationEvents.TryTake(out var integrationEvent))
            {
                var data = JsonConvert.SerializeObject(integrationEvent);
                var outboxMessage = new OutboxMessage(
                    integrationEvent.OccurredOn,
                    data);
                this.OutboxMessages.Add(outboxMessage);
            }
               
              
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishIntegrationEvents(Entity[] integrationEventsEntities, IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        if (integrationEventsEntities.Length == 0)
        {
            return;
        }
        _integrationEventPublisher.Transaction.Value = ActivatorUtilities.CreateInstance<SqlServerCapTransaction>(_integrationEventPublisher.ServiceProvider).Begin(transaction);

        foreach (var entity in integrationEventsEntities)
        {
            while (entity.IntegrationEvents.TryTake(out var integrationEvent))
            {
                await _integrationEventPublisher.PublishAsync(integrationEvent.Type, integrationEvent, cancellationToken: cancellationToken);
            }
        }
    }

    private async Task PublishDomainEventsAsync(Entity[] domainEventEntities, CancellationToken cancellationToken)
    {
        foreach (var entity in domainEventEntities)
        {
            while (entity.DomainEvents.TryTake(out var domainEvent))
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
