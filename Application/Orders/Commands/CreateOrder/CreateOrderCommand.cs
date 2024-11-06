using Application.Common.Attributes;
using Application.Common.Enums;
using Application.Orders.Commands.CreateOrderV2;
using Application.Security;

namespace Application.Orders.Commands.CreateOrder;

public record struct CreateOrderItemRequest(Guid ProductId, int Quantity);

[FeatureFlag(nameof(FeatureFlags.CreateOrder), typeof(CreateOrderCommandHandlerV2))]
[Authorize(Roles = Roles.Customer)]
public record CreateOrderCommand(IReadOnlyList<CreateOrderItemRequest> Items) : IAuthorizeRequest<ErrorOr<Guid>>;