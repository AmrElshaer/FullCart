using System.Reflection;
using Application.Common.Interfaces;
using Domain.Brands;
using Domain.Categories;
using Domain.Comments;
using Domain.Orders;
using Domain.Products;
using Domain.Roles;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Common.Persistence;



public class CartDbContext : DbContext, ICartDbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options)
        : base(options)
    {
    }

  

    public DbSet<Category> Categories { get; set; } = default!;

    public DbSet<Brand> Brands { get; set; } = default!;
    

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Comment> Comments { get; set; } = default!;

    public new DatabaseFacade Database => base.Database;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    // {
    //     var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
    //         .Select(po => po.Entity)
    //         .Where(po => po.DomainEvents.Any())
    //         .ToArray();
    //
    //     var integrationEventsEntities = ChangeTracker.Entries<Entity>()
    //         .Select(po => po.Entity)
    //         .Where(po => po.IntegrationEvents.Any());
    //  
    //     // ReSharper disable once PossibleMultipleEnumeration
    //     if (domainEventEntities.Length == 0 && integrationEventsEntities.ToArray().Length == 0)
    //         return await base.SaveChangesAsync(cancellationToken);
    //
    //     if (Database.CurrentTransaction is not null) // the transaction commit and rollback is managed outside
    //     {
    //         await PublishDomainEventsAsync(domainEventEntities, cancellationToken);
    //         var saveChangesResult = await base.SaveChangesAsync(cancellationToken);
    //         // ReSharper disable once PossibleMultipleEnumeration
    //         await PublishIntegrationEvents(integrationEventsEntities.ToArray(), Database.CurrentTransaction,
    //             cancellationToken);
    //
    //         return saveChangesResult;
    //     }
    //     
    //
    //     await using var transaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted,
    //         cancellationToken);
    //
    //     try
    //     {
    //         await PublishDomainEventsAsync(domainEventEntities, cancellationToken);
    //         // ReSharper disable once PossibleMultipleEnumeration
    //         await PublishIntegrationEvents(integrationEventsEntities.ToArray(), transaction, cancellationToken);
    //
    //         var result = await base.SaveChangesAsync(cancellationToken);
    //
    //         await transaction.CommitAsync(cancellationToken);
    //
    //         return result;
    //     }
    //     catch (Exception)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //
    //         throw;
    //     }
    // }

    // private async Task PublishIntegrationEvents(Entity[] integrationEventsEntities, IDbContextTransaction transaction,
    //     CancellationToken cancellationToken)
    // {
    //     if (integrationEventsEntities.Length == 0) return;
    //     _integrationEventPublisher.Transaction.Value = ActivatorUtilities
    //         .CreateInstance<SqlServerCapTransaction>(_integrationEventPublisher.ServiceProvider).Begin(transaction);
    //
    //     foreach (var entity in integrationEventsEntities)
    //         while (entity.IntegrationEvents.TryTake(out var integrationEvent))
    //             await _integrationEventPublisher.PublishAsync(integrationEvent.Type, integrationEvent,
    //                 cancellationToken: cancellationToken);
    // }
    //
    // private async Task PublishDomainEventsAsync(BaseEntity[] domainEventEntities, CancellationToken cancellationToken)
    // {
    //     foreach (var entity in domainEventEntities)
    //         while (entity.DomainEvents.TryTake(out var domainEvent))
    //             await _mediator.Publish(domainEvent, cancellationToken);
    // }
}