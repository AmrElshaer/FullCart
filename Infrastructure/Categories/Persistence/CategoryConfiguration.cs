using Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Categories.Persistence;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.OwnsOne(c => c.Name,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.Name)
                    .HasColumnName(nameof(CategoryName.Name))
                    .HasColumnType("nvarchar(50)");
            });
        builder.OwnsOne(c => c.FileName,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.FileName)
                    .HasColumnName(nameof(CategoryFileName.FileName))
                    .HasColumnType("nvarchar(50)");
            });
        
    }
}
