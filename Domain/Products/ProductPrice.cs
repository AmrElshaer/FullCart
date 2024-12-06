using BuildingBlocks.Domain.Common;
using ErrorOr;

namespace Domain.Products;

public class ProductPrice:ValueObject
{
    public decimal Price { get; private set; }

    private ProductPrice()
    {
        
    }

    private ProductPrice(decimal price)
    {
        Price = price;
    }

    public static ErrorOr<ProductPrice> Create(decimal price)
    {
        if (price < 0)
        {
            return Error.Validation("Product Price must be greater than zero");
        }

        return new ProductPrice(price);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Price;
    }
}
