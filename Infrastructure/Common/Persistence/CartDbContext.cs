using System.Data;
using System.Reflection;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Data;
using Domain.Brands;
using Domain.Categories;
using Domain.Comments;
using Domain.Common;
using Domain.Orders;
using Domain.Payments;
using Domain.Products;
using Domain.Roles;
using Domain.Users;
using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Persistence;

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

public class CartDbContext : ApplicationIdentityDbContext<User, Role, Guid>, ICartDbContext
{
    private readonly IMediator _mediator;
    private readonly ICapPublisher _integrationEventPublisher;

    public CartDbContext(DbContextOptions<CartDbContext> options, IMediator mediator,
        ICapPublisher integrationEventPublisher)
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
    public DbSet<Comment> Comments { get; set; } = default!;

    public new DatabaseFacade Database => base.Database;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        var integrationEventsEntities = ChangeTracker.Entries<Entity>()
            .Select(po => po.Entity)
            .Where(po => po.IntegrationEvents.Any())
            .ToList();
  
        if (domainEventEntities.Length == 0 && integrationEventsEntities.Count == 0)
            return await base.SaveChangesAsync(cancellationToken);

        if (Database.CurrentTransaction is not null) 
        {
            await PublishDomainEventsAsync(domainEventEntities, cancellationToken);
            var saveChangesResult = await base.SaveChangesAsync(cancellationToken);
            _integrationEventPublisher.Transaction = ActivatorUtilities.CreateInstance<SqlServerCapTransaction>(_integrationEventPublisher.ServiceProvider);
            _integrationEventPublisher.Transaction.DbTransaction = Database.CurrentTransaction;
            await PublishIntegrationEvents(integrationEventsEntities.ToArray(), cancellationToken);

            return saveChangesResult;
        }

        await using var transaction = await Database.BeginTransactionAsync(_integrationEventPublisher,cancellationToken:cancellationToken);

        try
        {
            await PublishDomainEventsAsync(domainEventEntities, cancellationToken);
            // ReSharper disable once PossibleMultipleEnumeration
            await PublishIntegrationEvents(integrationEventsEntities.ToArray(), cancellationToken);

            var result = await base.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }

    private async Task PublishIntegrationEvents(Entity[] integrationEventsEntities,
        CancellationToken cancellationToken)
    {
        if (integrationEventsEntities.Length == 0) return;
        foreach (var entity in integrationEventsEntities)
            while (entity.IntegrationEvents.TryTake(out var integrationEvent))
                await _integrationEventPublisher.PublishAsync(integrationEvent.Type, integrationEvent,
                    cancellationToken: cancellationToken);
    }

    private async Task PublishDomainEventsAsync(BaseEntity[] domainEventEntities, CancellationToken cancellationToken)
    {
        foreach (var entity in domainEventEntities)
            while (entity.DomainEvents.TryTake(out var domainEvent))
                await _mediator.Publish(domainEvent, cancellationToken);
    }
}