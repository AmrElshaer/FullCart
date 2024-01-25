using Application.Customers.Commands.CreateCustomer;
using Microsoft.AspNetCore.Mvc;

namespace FullCart.API.Controllers;

public class CustomerController:ApiController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCustomer(CreateCustomerCommand command)
        => (await Mediator.Send(command)).Match(r=>Ok(r), Problem);
}
    