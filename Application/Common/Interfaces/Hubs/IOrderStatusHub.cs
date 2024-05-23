using Domain.Orders;

namespace Application.Common.Interfaces.Hubs;

public interface IOrderStatusHub
{
    Task OrderStatusChanged(OrderStatus orderStatus);

}
