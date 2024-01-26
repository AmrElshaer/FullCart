using Domain.Common;
using ErrorOr;

namespace Domain.Brands;

public class BrandFileName:ValueObject
{
    public string FileName { get; private set; }= default!;

    private BrandFileName()
    {
        
    }

    private BrandFileName(string fileName)
    {
        FileName = fileName;
    }

    public static ErrorOr<BrandFileName>  Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Brand must have file name");
        }

        return new BrandFileName(name);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName;
    }
}
