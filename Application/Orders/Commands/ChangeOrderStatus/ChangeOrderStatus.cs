using BuildingBlocks.Application.Security;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatus
{
    public record Request(OrderStatus Status);

    [Authorize(Roles = Roles.Customer)]
    public record Command(Guid OrderId, OrderStatus OrderStatus) : IAuthorizeRequest<ErrorOr<Guid>>;

    internal class Handler : IRequestHandler<Command, ErrorOr<Guid>>
    {
        private readonly ICartDbContext _cartDbContext;

        public Handler(ICartDbContext cartDbContext)
        {
            _cartDbContext = cartDbContext;
        }

        public async Task<ErrorOr<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var order = await _cartDbContext.Orders.FirstOrDefaultAsync(o => o.Id == OrderId.Create(request.OrderId)
                , cancellationToken);

            if (order is null) return Error.NotFound($"Order with id {request.OrderId} not found");

            order.ChangeOrderStatus(request.OrderStatus);
            await _cartDbContext.SaveChangesAsync(cancellationToken);

            return order.Id.Value;
        }
    }
}