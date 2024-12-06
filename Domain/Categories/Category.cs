using BuildingBlocks.Domain.Common;

namespace Domain.Categories;

public class Category:Entity
{
    public CategoryName Name { get; private set; }= default!;

    public CategoryFileName FileName { get; private set; } = default!;
    private Category(){}

    public Category(Guid id,CategoryName name,CategoryFileName fileName)
    {
        Id = id;
        Name = name;
        FileName = fileName;
    }

    public void Update(CategoryName name, CategoryFileName fileName)
    {
        Name = name;
        FileName = fileName;
    }
}
