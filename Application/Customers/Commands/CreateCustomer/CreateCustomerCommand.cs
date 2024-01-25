using FluentValidation;
using MediatR;

namespace Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.City).NotEmpty()
            .WithMessage("City is required");
    }
}
public class CreateCustomerCommand:IRequest<ErrorOr.ErrorOr<Guid>>
{
    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;

    public string? Street { get; init; }

    public string City { get; init; } = default!;
    public string? State { get; init; }
}
