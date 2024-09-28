using Application.Common.Interfaces.Data;
using Bogus;
using Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Categories.Persistence;

public class CategoryDataSeeder(ICartDbContext cartDbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await cartDbContext.Categories.AnyAsync()) return;
        await cartDbContext.Categories.AddRangeAsync(InitialDataSeeder.GetCategories());
        await cartDbContext.SaveChangesAsync(default);
    }
}

internal static class InitialDataSeeder
{
    public static IReadOnlyList<Category> GetCategories()
    {
        return Enumerable.Range(1, 10)
            .Select(i => new Category(Guid.NewGuid(),
                CategoryName.Create($"Category_{i}").Value,
                CategoryFileName.Create($"file_{i}").Value)).ToList();
    }
}