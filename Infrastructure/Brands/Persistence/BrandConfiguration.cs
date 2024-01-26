using Domain.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Brands.Persistence;

public class BrandConfiguration: IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.OwnsOne(c => c.Name,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.Name)
                    .HasColumnName(nameof(BrandName.Name))
                    .HasColumnType("nvarchar(50)");
            });
        builder.OwnsOne(c => c.FileName,
            ownBuilder =>
            {

                ownBuilder.Property(p => p.FileName)
                    .HasColumnName(nameof(BrandFileName.FileName))
                    .HasColumnType("nvarchar(50)");
            });
    }
}
