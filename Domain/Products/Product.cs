using Domain.Brands;
using Domain.Categories;
using Domain.Common;

namespace Domain.Products;

public class Product : Entity
{
    public ProductName Name { get; private set; } = default!;

    public ProductDescription Description { get; private set; } = default!;

    public ProductPrice Price { get; private set; } = default!;

    public ProductFileName FileName { get; private set; } = default!;

    public Guid BrandId { get; private set; }

    public Brand Brand { get; private set; } = default!;

    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; } = default!;

    public ProductQuantity ProductQuantity { get; private set; } = default!;


    private Product()
    {
    }

    public Product
    (
        Guid id,
        ProductName name,
        ProductDescription description,
        ProductPrice price,
        ProductFileName fileName,
        ProductQuantity productQuantity,
        Guid brandId,
        Guid categoryId
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        FileName = fileName;
        ProductQuantity = productQuantity;
        BrandId = brandId;
        CategoryId = categoryId;
    }

    public void UpdateQuantity(ProductQuantity productQuantity)
    {
        ProductQuantity = productQuantity;
    }
}