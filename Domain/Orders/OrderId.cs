using Domain.Comments;
using Domain.Common;

namespace Domain.Orders;

public class OrderId : ValueObject
{
    public Guid Value { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static OrderId CreateUniqueId()
    {
        return Create(
            Guid.NewGuid()
        );
    }

    public static OrderId Create(Guid value)
    {
        return new OrderId()
        {
            Value = value
        };
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}