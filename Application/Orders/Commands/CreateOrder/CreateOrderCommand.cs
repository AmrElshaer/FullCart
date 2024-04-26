using Application.Security;
using Domain.Roles;
using ErrorOr;
using FluentValidation;

namespace Application.Orders.Commands.CreateOrder;

[Authorize(Roles = Roles.Customer)]
public record CreateOrderCommand(IReadOnlyList<CreateOrderItemRequest> Items) : IAuthorizeRequest<ErrorOr<Guid>>;

public record CreateOrderItemRequest(Guid ProductId, int Quantity);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(r => r.Items)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Items)
            .Must(items => items.Select(item => item.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate products are not allowed.");
    }
}
