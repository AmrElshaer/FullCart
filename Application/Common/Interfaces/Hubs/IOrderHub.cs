using Domain.Orders;

namespace Application.Common.Interfaces.Hubs;

public interface IOrderHub
{
  
    void SendOrderStatusChanged(Guid notificationOrderId, OrderStatus notificationOrderStatus);
}
