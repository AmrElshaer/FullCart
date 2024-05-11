using Application.Orders.Commands.CreateOrder;
using Application.Orders.Queries.GetOrderById;
using Domain.Orders;
using Microsoft.AspNetCore.Mvc;
namespace FullCart.API.Controllers;

public class OrderController:ApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> Get(Guid id)
        => (await Mediator.Send(new GetOrderByIdQuery(id))).Match(Ok, Problem); 
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(CreateOrder.Command command)
        => (await Mediator.Send(command)).Match(r=>Ok(r), Problem);
}
