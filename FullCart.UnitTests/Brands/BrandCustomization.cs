using AutoFixture;
using Domain.Brands;
using FullCart.UnitTests.Common;
using Microsoft.EntityFrameworkCore;

namespace FullCart.UnitTests.Brands;

public class BrandCustomization : IFixtureCustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Brand>(composer => composer.FromFactory(() => new Brand(
            fixture.Create<Guid>(),
            BrandName.Create(fixture.Create<string>()).Value,
            BrandFileName.Create(fixture.Create<string>()).Value
        )));

        fixture.Customize<DbSet<Brand>>(composer => composer.FromFactory(() =>
        {
            var brands = fixture.CreateMany<Brand>().ToList();
            return brands.AsQueryable().BuildMockDbSet();
        }));
    }
}