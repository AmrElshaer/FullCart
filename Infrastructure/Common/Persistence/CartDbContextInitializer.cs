using Application.Common.Interfaces.Data;
using Domain.Roles;
using Domain.Users;
using Infrastructure.Common.Persistence.Seeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Common.Persistence;

public static class InitializerExtensions
{
    public static IApplicationBuilder InitialiseDatabase(this IApplicationBuilder app)
    {
        MigrateDatabaseAsync<CartDbContext>(app.ApplicationServices).GetAwaiter().GetResult();

        SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
        return app;
    }

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider)
        where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seederExecutor = scope.ServiceProvider.GetRequiredService<SeederExecutor>();
        await seederExecutor.ExecuteAllAsync();
    }
}

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Error happen `{Ex}`")]
    public static partial void LogException(this ILogger logger, Exception ex);
}