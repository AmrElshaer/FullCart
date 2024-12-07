using Application.Common.Interfaces;
using Application.Tokens.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FullCart.API.Controllers;

public class TokenController:ApiController
{
    private readonly IOrderModule _orderModule;

    public TokenController(IOrderModule orderModule)
    {
        _orderModule = orderModule;
    }
    [HttpPost("generate-token")]
    public async Task<ActionResult<GenerateTokenResponse>> GenerateToken(GenerateTokenQuery query)
        => (await _orderModule.SendAsync(query)).Match(Ok, Problem);
}
