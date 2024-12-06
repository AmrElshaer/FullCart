using BuildingBlocks.Domain.Common;

namespace Payment.Contracts.Events.IntegrationEvents;

public class PaymentCreatedNotification : IntegrationEvent
{
    public Guid PaymentId { get; }

    public PaymentCreatedNotification(Guid paymentId)
    {
        PaymentId = paymentId;
        Type = IntegrationEventConstants.PaymentConstant.PaymentCreated;
    }
}