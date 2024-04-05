using Domain.Payments.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payments.Commands.CreatePayment
{
    public class PaymentCreatedNotificationHandler : INotificationHandler<PaymentCreatedNotification>
    {
        private readonly ILogger<PaymentCreatedNotificationHandler> _logger;

        public PaymentCreatedNotificationHandler(ILogger<PaymentCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(PaymentCreatedNotification request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PaymentCreatedNotificationHandler: {request}", request);
            await Task.CompletedTask;
        }
    }

}
