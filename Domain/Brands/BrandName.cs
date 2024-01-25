using Domain.Common;
using ErrorOr;

namespace Domain.Brands;

public class BrandName:ValueObject
{
    public string Name { get; private set; }

    private BrandName(string name)
    {
        Name = name;
    }

    public static ErrorOr<BrandName>  Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Brand must have name");
        }

        return new BrandName(name);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
