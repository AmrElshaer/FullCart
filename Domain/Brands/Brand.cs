using Domain.Common;

namespace Domain.Brands;

public class Brand:Entity
{
    public BrandName Name { get; private set; } = default!;

    public BrandFileName FileName { get; private set; } = default!;
   
    private Brand(){}
    public Brand(Guid id,BrandName name,BrandFileName fileName)
    {
        Id = id;
        Name = name;
        FileName = fileName;
    }

    public void Update(BrandName name, BrandFileName fileName)
    {
        Name = name;
        FileName = fileName;
    }
}
