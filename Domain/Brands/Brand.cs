using Domain.Common;

namespace Domain.Brands;

public class Brand:Entity
{
    public BrandName Name { get; private set; }

    public BrandFileName FileName { get; private set; } 

    private Brand(Guid id,BrandName name,BrandFileName fileName)
    {
        Id = id;
        Name = name;
        FileName = fileName;
    }

   
}
