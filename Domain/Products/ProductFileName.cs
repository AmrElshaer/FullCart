using BuildingBlocks.Domain.Common;
using ErrorOr;

namespace Domain.Products;

public class ProductFileName:ValueObject
{
    public string FileName { get; private set; }= default!;
    private ProductFileName(){}

    private ProductFileName(string description)
    {
        FileName = description;
    }

    public static ErrorOr<ProductFileName>  Create(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Error.Validation("Product fileName must have value");
        }

        return new ProductFileName(fileName);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName;
    }
}
