using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class NumberOfOrdersJob(IServiceProvider serviceProvider, ILogger<NumberOfOrdersJob> logger) : BackgroundService
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(1));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ICartDbContext>();
                var orderNumbers = await dbContext.Orders.CountAsync(stoppingToken);
                logger.LogInformation("Number of orders {0}", orderNumbers);
                await Task.Delay(10000);
            }
    }
}