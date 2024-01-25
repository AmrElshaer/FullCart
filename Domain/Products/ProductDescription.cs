using Domain.Common;
using ErrorOr;

namespace Domain.Products;

public class ProductDescription:ValueObject
{
    public string Description { get; private set; }

    private ProductDescription(string description)
    {
        Description = description;
    }

    public static ErrorOr<ProductDescription>  Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Error.Validation("Product description must have value");
        }

        return new ProductDescription(description);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Description;
    }
}
