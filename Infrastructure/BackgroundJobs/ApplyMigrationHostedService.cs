using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class ApplyMigrationHostedService:BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ApplyMigrationHostedService> _logger;

    public ApplyMigrationHostedService(IServiceProvider serviceProvider,ILogger<ApplyMigrationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Start Migration check");
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ICartDbContext>();
                await Task.Delay(1000*60*2, stoppingToken);
                await dbContext.Database.MigrateAsync(stoppingToken);
                _logger.LogInformation("Finish check");
            }
          
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "An error occurred while initialising the database");

            throw;
        }
    }
}
