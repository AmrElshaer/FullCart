using Domain.Comments.Events;
using Domain.Common;
using Domain.Orders;

namespace Domain.Comments;

public class Comment : BaseEntity<CommentId>
{
    private Comment()
    {
    }

    public OrderId OrderId { get; set; } = default!;
    public string Content { get; init; } = null!;

    public static Comment Create(OrderId orderId, string content)
    {
        var comment = new Comment
        {
            Id = CommentId.CreateUniqueId(),
            Content = content,
            OrderId = orderId
        };
        comment.AddDomainEvent(new CommentCreated
        {
            OrderId = orderId,
            CommentId = comment.Id
        });
        return comment;
    }
}