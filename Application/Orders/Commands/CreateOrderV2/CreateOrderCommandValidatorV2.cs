namespace Application.Orders.Commands.CreateOrderV2;

public class CreateOrderCommandValidatorV2 : AbstractValidator<CreateOrderCommandV2>
{
    public CreateOrderCommandValidatorV2()
    {
        RuleFor(r => r.Items)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Items)
            .Must(items => items.Select(item => item.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate products are not allowed.");
    }
}