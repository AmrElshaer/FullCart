using Domain.Common;
using ErrorOr;

namespace Domain.Products;

public class ProductName:ValueObject
{
    public string Name { get; private set; }

    private ProductName(string name)
    {
        Name = name;
    }

    public static ErrorOr<ProductName>  Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Product name must have value");
        }

        return new ProductName(name);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
