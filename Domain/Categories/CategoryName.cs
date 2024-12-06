using BuildingBlocks.Domain.Common;
using ErrorOr;

namespace Domain.Categories;

public class CategoryName:ValueObject
{
    public string Name { get; private set; }= default!;

    private CategoryName()
    {
        
    }
    private CategoryName(string name)
    {
        Name = name;
    }

    public static ErrorOr<CategoryName>  Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Category must have file name");
        }

        return new CategoryName(name);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
