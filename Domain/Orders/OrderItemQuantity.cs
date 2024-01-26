using Domain.Common;
using ErrorOr;

namespace Domain.Orders;

public class OrderItemQuantity:ValueObject
{
    public int Quantity { get; private set; }

    private OrderItemQuantity()
    {
        
    }

    private OrderItemQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public static ErrorOr<OrderItemQuantity> Create(int quantity)
    {
        if (quantity<0)
        {
            return Error.Validation("Quantity must be greater than zero");
        }

        if (quantity>100)
        {
            return Error.Validation("Quantity must be less than or equal to 100");
        }

        return new OrderItemQuantity(quantity);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Quantity;
    }
}
