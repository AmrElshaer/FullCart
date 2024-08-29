using System.Text.Json;
using Domain.Payments.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Payments.Consumer;

public class PaymentConsumer : IConsumer<PaymentCreated>
{
    private readonly ILogger<PaymentConsumer> _logger;
    private readonly IConfiguration _configuration;

    public PaymentConsumer(ILogger<PaymentConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<PaymentCreated> context)
    {
        var serviceName = _configuration["PaymentConsumerConfiguration:ServiceName"];

        _logger.LogInformation(message: "order payment created payment id{0} from {1}"
            , context.Message.PaymentId, serviceName);
        await Task.CompletedTask;
    }
}
