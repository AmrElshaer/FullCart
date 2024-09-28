using Domain.Common;
using ErrorOr;

namespace Domain.Products;

public class ProductQuantity : ValueObject
{
    public int Value { get; private set; }

    private ProductQuantity()
    {
    }

    private ProductQuantity(int value)
    {
        Value = value;
    }

    public static ErrorOr<ProductQuantity> Create(int value)
    {
        if (value < 0) return Error.Validation("Value must be greater than or equal to 0.");
        return new ProductQuantity(value);
    }

    public static ProductQuantity operator -(ProductQuantity quantity1, ProductQuantity quantity2)
    {
        ArgumentNullException.ThrowIfNull(quantity2);

        var newValue = quantity1.Value - quantity2.Value;

        return new ProductQuantity(newValue);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}