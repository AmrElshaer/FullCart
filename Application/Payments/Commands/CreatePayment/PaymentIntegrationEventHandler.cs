using Domain.Common;
using Domain.Payments.Events;
using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Commands.CreatePayment
{
    public class PaymentIntegrationEventHandler : INotificationHandler<PaymentCreatedNotification>
    {
        private readonly ILogger<PaymentIntegrationEventHandler> _logger;

        public PaymentIntegrationEventHandler(ILogger<PaymentIntegrationEventHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task Handle(PaymentCreatedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PaymentCreatedNotificationHandler: {@Notification}", notification);
            await Task.Delay(2000);
            await Task.CompletedTask;
        }
    }
}
