using Application.Common.Interfaces;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace ScallingBackground.Jobs;

public interface IOrderJob
{
    Task UpdateOrderStatus();
}

public class OrderJob : IOrderJob
{
    private readonly ICartDbContext _cartDbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderJob> _logger;

    public OrderJob(ICartDbContext cartDbContext,IConfiguration configuration,ILogger<OrderJob> logger)
    {
        _cartDbContext = cartDbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task UpdateOrderStatus()
    {
        try
        {
            var serviceName = _configuration.GetValue<string>("Service:Name");
            _logger.LogInformation($"Update Orders Background job service {serviceName}");
            var orders =  _cartDbContext.Orders.Where(o => o.Status == OrderStatus.Pending)
                .ToList();

            foreach (var order in orders)
            {
                order.ChangeOrderStatus(OrderStatus.Confirmed);
            }

            _cartDbContext.SaveChangesAsync(default).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw;
        }
    }
    
}
public class OrderJobWrapper
{
    private readonly IServiceProvider _serviceProvider;

    public OrderJobWrapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void UpdateOrderStatus()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var orderJob = scope.ServiceProvider.GetRequiredService<IOrderJob>();
            orderJob.UpdateOrderStatus();
        }
    }
}