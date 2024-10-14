using Application.Common.Interfaces.Data;
using Domain.Comments.Events;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.EventHandlers;

public class OrderCommentCreatedEventHandler(ICartDbContext cartDbContext) : INotificationHandler<CommentCreated>
{
    public async Task Handle(CommentCreated notification, CancellationToken cancellationToken)
    {
        var order = await cartDbContext.Orders.FirstOrDefaultAsync(o => o.Id == notification.OrderId
            , cancellationToken);
        ArgumentNullException.ThrowIfNull(order);
        order.AddComments(notification.CommentId);
    }
}