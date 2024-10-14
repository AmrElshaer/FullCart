using Domain.Common;
using Domain.Orders;

namespace Domain.Comments.Events;

public class CommentCreated : DomainEvent
{
    public required OrderId OrderId { get; init; }

    public required CommentId CommentId { get; init; }
}