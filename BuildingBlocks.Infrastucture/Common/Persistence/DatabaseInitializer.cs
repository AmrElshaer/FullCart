using Autofac;
using BuildingBlocks.Infrastucture.Common.Persistence.Seeder;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastucture.Common.Persistence;

public static class InitializerExtensions
{
    public static void InitialiseDatabase<TContext>(this IContainer container)
        where TContext : DbContext
    {
        MigrateDatabaseAsync<TContext>(container).GetAwaiter().GetResult();
        SeedDataAsync(container).GetAwaiter().GetResult();
    }

    private static async Task MigrateDatabaseAsync<TContext>(IContainer container)
        where TContext : DbContext
    {
        await using var scope = container.BeginLifetimeScope();
        var context = scope.Resolve<TContext>();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedDataAsync(IContainer container)
    {
        await using var scope = container.BeginLifetimeScope();
        var seederExecutor = scope.Resolve<SeederExecutor>();
        await seederExecutor.ExecuteAllAsync();
    }
}