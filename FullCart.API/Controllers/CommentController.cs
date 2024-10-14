using Application.Comments.Commands.CreateComment;
using Application.Orders.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc;

namespace FullCart.API.Controllers;

public class CommentController : ApiController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateComment(CreateCommentCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}