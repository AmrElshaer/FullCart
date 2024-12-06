using BuildingBlocks.Domain.Common;
using Contracts.Events;
using Domain.Comments;
using Domain.Orders.Events;
using Domain.Users;

namespace Domain.Orders;

public class Order : BaseEntity<OrderId>
{
    private readonly List<CommentId> _commentIds = new();

    private readonly List<OrderItem> _items = new();

    private Order()
    {
    }

    public Order(Guid customerId, IReadOnlyList<OrderItem> items, DateTimeOffset creationDate)
    {
        Id = OrderId.CreateUniqueId();
        Status = OrderStatus.Pending;
        TotalPrice = items.Sum(i => i.ProductPrice.Price);
        CustomerId = customerId;
        CreationDate = creationDate;
        _items.AddRange(items);
        AddDomainEvent(new OrderPlacedEvent
        {
            OrderId = Id.Value,
            CustomerId = customerId
        });
        AddIntegrationEvent(new OrderPlacedIntegrationEvent(Id));
    }

    public OrderStatus Status { get; private set; }

    public decimal TotalPrice { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    public Guid CustomerId { get; }

    public DateTimeOffset CreationDate { get; private set; }

    public Customer Customer { get; set; } = default!;

    public IReadOnlyCollection<CommentId> CommentIds => _commentIds;

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