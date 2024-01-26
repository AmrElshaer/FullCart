using Domain.Common;
using ErrorOr;

namespace Domain.Categories;

public class CategoryFileName:ValueObject
{
    public string FileName { get; private set; }= default!;

    private CategoryFileName()
    {
        
    }

    private CategoryFileName(string fileName)
    {
        FileName = fileName;
    }

    public static ErrorOr<CategoryFileName>  Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Category must have name");
        }

        return new CategoryFileName(name);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName;
    }
}
