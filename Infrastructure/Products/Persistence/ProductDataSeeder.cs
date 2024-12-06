using Application.Common.Interfaces;
using BuildingBlocks.Application.Common.Interfaces.Data;
using Domain.Products;
using Infrastructure.Brands.Persistence;
using Infrastructure.Categories.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Products.Persistence;

public class ProductDataSeeder(ICartDbContext context) : IDataSeeder
{
    public IEnumerable<Type> GetDependentDataSeeders()
    {
        return [typeof(BrandDataSeeder), typeof(CategoryDataSeeder)];
    }

    public async Task SeedAllAsync()
    {
        if (await context.Products.AnyAsync()) return;
        var brand = await context.Brands.FirstAsync();
        var category = await context.Categories.FirstAsync();
        await context.Products.AddRangeAsync(InitialDataSeeder.GetProducts(brand.Id, category.Id));
        await context.SaveChangesAsync(default);
    }
}

internal static class InitialDataSeeder
{
    public static IEnumerable<Product> GetProducts(Guid brandId, Guid categoryId)
    {
        return Enumerable.Range(1, 10)
            .Select(i => new Product(Guid.NewGuid(),
                ProductName.Create($"Product_{i}").Value,
                ProductDescription.Create($"Description_{i}").Value,
                ProductPrice.Create(200).Value,
                ProductFileName.Create($"file_{i}").Value,
                ProductQuantity.Create(i * 200).Value
                , brandId, categoryId)).ToList();
    }
}