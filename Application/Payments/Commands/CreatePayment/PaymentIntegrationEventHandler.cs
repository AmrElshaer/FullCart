using Domain.Common;
using Domain.Payments.Events;
using DotNetCore.CAP;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Commands.CreatePayment;

public class PaymentIntegrationEventHandler : ICapSubscribe
{
    private readonly ILogger<PaymentIntegrationEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentIntegrationEventHandler
    (
        ILogger<PaymentIntegrationEventHandler> logger,
        IPublishEndpoint publishEndpoint
    )
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [CapSubscribe(IntegrationEventConstants.PaymentConstant.PaymentCreated)]
    public async Task HandleAsync(PaymentCreatedNotification notification)
    {
        _logger.LogInformation(message: "PaymentCreatedNotificationHandler: {@Notification}", notification);
        await _publishEndpoint.Publish<PaymentCreated>(new PaymentCreated(notification.PaymentId));
    }
}
