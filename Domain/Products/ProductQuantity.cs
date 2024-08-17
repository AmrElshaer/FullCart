using Domain.Common;

namespace Domain.Products;

public class ProductQuantity : ValueObject
{
    public int Value { get; private set; }

    private ProductQuantity() { }

    public ProductQuantity(int value)
    {
        Value = value;
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
