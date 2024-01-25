using Domain.Brands;
using Domain.Categories;
using Domain.Common;

namespace Domain.Products;

public class Product:Entity
{
    public ProductName Name { get; private set; }

    public ProductDescription Description { get; private set; }

    public ProductPrice Price { get; private set; }

    public ProductFileName FileName { get; private set; }
    public Guid BrandId { get; private set; }

    public Brand Brand { get; private set; } = default!;

    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; } = default!;

    public Product(Guid id,ProductName name,ProductDescription description,
        ProductPrice price,ProductFileName fileName,Guid brandId,Guid categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        FileName = fileName;
        BrandId = brandId;
        CategoryId = categoryId;
    }
    
    
}
