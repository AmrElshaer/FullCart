using Medallion.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class DistributedLockTestJob(IDistributedLockProvider distributedLockProvider,ILogger<DistributedLockTestJob> logger): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        while (!stoppingToken.IsCancellationRequested)
        {
            var @lock =  distributedLockProvider.CreateLock("OrdersLock");
            // AcquireAsync timout for the new acquire request to wait unital throw exception if lock is not available
            await using var handler=await @lock.TryAcquireAsync(TimeSpan.FromSeconds(2),stoppingToken);
            if (handler is null)
            {
                logger.LogWarning("Another process is holding the lock. Waiting for it to release...");
            }
            else
            {
                logger.LogInformation("Lock acquired. Executing critical section...{Date}",DateTime.Now);
                await Task.Delay(TimeSpan.FromMinutes(1),stoppingToken); 
                logger.LogInformation("Critical section complete. Releasing lock...{Date}",DateTime.Now);
            }
        }
    }
}