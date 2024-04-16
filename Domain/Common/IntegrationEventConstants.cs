namespace Domain.Common;

public class IntegrationEventConstants
{
    private const string Events = "events:";

    public static class PaymentConstant
    {
        public const string PaymentCreated = $"{Events}payment_created";
    }

    public static class OrderConstant
    {
        public const string OrderPlaced = $"{Events}order_placed";
    }
}
