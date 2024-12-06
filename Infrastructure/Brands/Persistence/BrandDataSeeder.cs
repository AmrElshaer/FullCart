using Application.Common.Interfaces;
using BuildingBlocks.Application.Common.Interfaces.Data;
using Domain.Brands;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Brands.Persistence;

public class BrandDataSeeder(ICartDbContext context) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await context.Brands.AnyAsync()) return;

        await context.Brands.AddRangeAsync(InitialDataSeeder.GetBrands());
        await context.SaveChangesAsync(default);
    }
}

internal static class InitialDataSeeder
{
    public static IReadOnlyList<Brand> GetBrands()
    {
        return Enumerable.Range(1, 10)
            .Select(i => new Brand(Guid.NewGuid(),
                BrandName.Create($"Brand_{i}").Value,
                BrandFileName.Create($"file_{i}").Value)).ToList();
    }
}