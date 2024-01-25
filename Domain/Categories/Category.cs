using Domain.Common;

namespace Domain.Categories;

public class Category:Entity
{
    public CategoryName Name { get; private set; }

    public CategoryFileName FileName { get; private set; } 

    private Category(Guid id,CategoryName name,CategoryFileName fileName)
    {
        Id = id;
        Name = name;
        FileName = fileName;
    }

   
}
