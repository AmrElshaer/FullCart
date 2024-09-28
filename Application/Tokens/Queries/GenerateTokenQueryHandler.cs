using Application.Common.Interfaces;
using Application.Common.Interfaces.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Tokens.Queries;

public class GenerateTokenQueryHandler : IRequestHandler<GenerateTokenQuery
    , ErrorOr<GenerateTokenResponse>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IIdentityService _identityService;

    public GenerateTokenQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IIdentityService identityService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _identityService = identityService;
    }

    public async Task<ErrorOr<GenerateTokenResponse>> Handle(GenerateTokenQuery request,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        if (email.IsError) return email.Errors;

        var user = await _identityService.GetUserAsync(email.Value);

        if (user.IsError) return user.Errors;

        var token = _jwtTokenGenerator.GenerateToken(user.Value.Id,
            user.Value.Email, user.Value.UserType, user.Value.Roles.ToList());

        return new GenerateTokenResponse(token);
    }
}