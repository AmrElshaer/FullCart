using ErrorOr;
using FluentValidation;
using MediatR;

namespace Application.Tokens.Queries;
public class GenerateTokenQueryValidator: AbstractValidator<GenerateTokenQuery>
{
    public GenerateTokenQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

    }
}
public record GenerateTokenQuery
    (string Email, string Password):IRequest<ErrorOr<GenerateTokenResponse>>;

