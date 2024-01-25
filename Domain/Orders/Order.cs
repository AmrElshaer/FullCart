using Domain.Common;
using Domain.Users;

namespace Domain.Orders;

public class Order:Entity
{
    public OrderStatus Status { get; private set; }

    public decimal TotalPrice { get; private set; }

    private readonly List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items;

    public Guid CustomerId { get; private set; }

    public Customer Customer { get; set; } = default!;

    public Order(Guid id,Guid customerId,IReadOnlyList<OrderItem> items)
    {
        Id = id;
        Status = OrderStatus.Pending;
        TotalPrice = items.Sum(i => i.ProductPrice.Price);
        CustomerId = customerId;
       _items.AddRange(items);
       
    }
    
}
