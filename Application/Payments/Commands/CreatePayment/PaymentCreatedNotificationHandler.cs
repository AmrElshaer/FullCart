using Domain.Payments.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Commands.CreatePayment
{
    public class PaymentCreatedNotificationHandler : INotificationHandler<PaymentCreatedNotification>
    {
        private readonly ILogger<PaymentCreatedNotificationHandler> _logger;

        public PaymentCreatedNotificationHandler(ILogger<PaymentCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(PaymentCreatedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PaymentCreatedNotificationHandler: {notification}", notification);
            await Task.CompletedTask;
        }
    }
}
