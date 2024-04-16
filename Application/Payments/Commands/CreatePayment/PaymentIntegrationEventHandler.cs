using Domain.Common;
using Domain.Payments.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Commands.CreatePayment
{
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
            await Task.CompletedTask;
        }
    }
}
