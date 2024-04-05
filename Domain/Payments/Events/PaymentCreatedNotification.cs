using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Payments.Events
{
    public class PaymentCreatedNotification : IntegrationEvent<PaymentCreatedEvent>
    {
        public Guid PaymentId { get; }

        public PaymentCreatedNotification(PaymentCreatedEvent domainEvent) : base(domainEvent)
        {
            this.PaymentId = domainEvent.PaymentId;
        }

        [JsonConstructor]
        public PaymentCreatedNotification(Guid paymentId) : base(null)
        {
            this.PaymentId = paymentId;
        }
    }
}
