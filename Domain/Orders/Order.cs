using Domain.Comments;
using Domain.Common;
using Domain.Orders.Events;
using Domain.Users;

namespace Domain.Orders;

public class Order : BaseEntity<OrderId>
{
    public OrderStatus Status { get; private set; }

    public decimal TotalPrice { get; private set; }

    private readonly List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items;

    public Guid CustomerId { get; private set; }

    public DateTimeOffset CreationDate { get; private set; }

    public Customer Customer { get; set; } = default!;

    private Order()
    {
    }

    private readonly List<CommentId> _commentIds = new();

    public IReadOnlyCollection<CommentId> CommentIds => _commentIds;

    public Order(Guid customerId, IReadOnlyList<OrderItem> items, DateTimeOffset creationDate)
    {
        Id = OrderId.CreateUniqueId();
        Status = OrderStatus.Pending;
        TotalPrice = items.Sum(i => i.ProductPrice.Price);
        CustomerId = customerId;
        CreationDate = creationDate;
        _items.AddRange(items);
        AddDomainEvent(new OrderPlacedEvent()
        {
            OrderId = Id,
            CustomerId = customerId
        });
        AddIntegrationEvent(new OrderPlacedIntegrationEvent(Id));
    }

    public void AddComments(CommentId commentId)
    {
        _commentIds.Add(commentId);
    }

    public string ToOrderTrackingGroupId()
    {
        return $"{Id}:{CustomerId}";
    }

    public void ChangeOrderStatus(OrderStatus requestOrderStatus)
    {
        Status = requestOrderStatus;
        AddIntegrationEvent(new OrderStatusChangeIntegrationEvent(Id, Status));
    }
}