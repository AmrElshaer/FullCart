using BuildingBlocks.Domain.Common;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Payment.Contracts.Events.IntegrationEvents;

namespace Payment.Application.Integrations;

public class PaymentIntegrationEventHandler : ICapSubscribe
{
    private readonly ILogger<PaymentIntegrationEventHandler> _logger;

    public PaymentIntegrationEventHandler(ILogger<PaymentIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    [CapSubscribe(IntegrationEventConstants.PaymentConstant.PaymentCreated)]
    public async Task HandleAsync(PaymentCreatedNotification notification)
    {
        _logger.LogInformation("PaymentCreatedNotificationHandler: {@Notification}", notification);
        await Task.Delay(2000);
        await Task.CompletedTask;
    }
}