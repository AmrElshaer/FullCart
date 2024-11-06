using Application.Security;

namespace Application.Orders.Commands.CreateOrderV2;

public record struct CreateOrderItemRequest(Guid ProductId, int Quantity);

[Authorize(Roles = Roles.Customer)]
public class CreateOrderCommandV2 : IAuthorizeRequest<ErrorOr<Guid>>
{
    public IReadOnlyList<CreateOrderItemRequest> Items { get; init; }
}