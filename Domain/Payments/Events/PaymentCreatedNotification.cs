using Domain.Common;

namespace Domain.Payments.Events
{
    public class PaymentCreatedNotification : IntegrationEvent
    {
        public Guid PaymentId { get; }

        public PaymentCreatedNotification(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
