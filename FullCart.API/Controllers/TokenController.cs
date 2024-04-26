using Application.Tokens.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FullCart.API.Controllers;

public class TokenController : ApiController
{
    [HttpPost("generate-token")]
    public async Task<ActionResult<GenerateTokenResponse>> GenerateToken(GenerateTokenQuery query)
        => (await Mediator.Send(query)).Match(Ok, Problem);
}
