using Application.Common.Interfaces;
using Domain.Orders;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<ErrorOr<Order>>;
public class GetOrderByIdQueryHandler:IRequestHandler<GetOrderByIdQuery,ErrorOr<Order>>
{
    private readonly ICartDbContext _cartDbContext;

    public GetOrderByIdQueryHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }
    public async Task<ErrorOr<Order>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _cartDbContext.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId,cancellationToken);

        if (order is null)
        {
            return Error.NotFound($"not found order with id {request.OrderId}");
        }

        return order;
    }
}
