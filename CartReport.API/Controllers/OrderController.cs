using Microsoft.AspNetCore.Mvc;

namespace CartReport.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IHttpClientFactory clientFactory, ILogger<OrderController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var httpClient = _clientFactory.CreateClient("CartAPI");

        _logger.LogInformation("url is {url}", httpClient.BaseAddress);
        var response = await httpClient.GetAsync($"/api/order/{id}");
        response.EnsureSuccessStatusCode();
        // Log the received order number
        _logger.LogInformation("Received Order Number: {OrderNumber}", id);

        return Ok(new { message = $"Order {id} logged successfully." });
    }
}