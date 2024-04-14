using Domain.Common;

namespace Domain.Payments.Events
{
    public class PaymentCreatedEvent : DomainEvent
    {
        public Guid PaymentId { get; init; }

        public Guid OrderId { get; init; }
    }
}
