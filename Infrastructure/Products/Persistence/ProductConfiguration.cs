using Domain.Brands;
using Domain.Categories;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Products.Persistence;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.OwnsOne(c => c.Name,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.Name)
                    .HasColumnName(nameof(ProductName.Name))
                    .HasColumnType("nvarchar(50)");
            });
        builder.OwnsOne(c => c.Description,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.Description)
                    .HasColumnName(nameof(ProductDescription.Description))
                    .HasColumnType("nvarchar(50)");
            });
        builder.OwnsOne(c => c.Price,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.Price)
                    .HasColumnName(nameof(ProductPrice.Price))
                    .HasColumnType("decimal(18,4)");
            });
        builder.OwnsOne(c => c.FileName,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.FileName)
                    .HasColumnName(nameof(ProductFileName.FileName))
                    .HasColumnType("nvarchar(50)");
            });

        builder.HasOne<Category>(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne<Brand>(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
