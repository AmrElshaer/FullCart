using Application.Common.Interfaces;
using Application.Orders.Commands.ChangeOrderStatus;
using Application.Orders.Commands.CreateOrder;
using Application.Orders.Queries.GetOrderById;
using Domain.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FullCart.API.Controllers;

public class OrderController:ApiController
{
    private readonly IOrderModule _orderModule;

    public OrderController(IOrderModule orderModule)
    {
        _orderModule = orderModule;
    }
    [HttpGet("{id:guid}")]
    [EnableRateLimiting("fixed")]
    public async Task<ActionResult<Order>> Get(Guid id)
        => (await Mediator.Send(new GetOrderByIdQuery(id))).Match(Ok, Problem); 
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(CreateOrder.Command command)
        => (await _orderModule.SendAsync(command)).Match(r=>Ok(r), Problem);
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> EditOrder(Guid id,[FromBody] ChangeOrderStatus.Request orderStatusRequest)
    {
      
        var command = new ChangeOrderStatus.Command(id,orderStatusRequest.Status);
        return (await Mediator.Send(command)).Match(r => Ok(r), Problem);
    }
}
