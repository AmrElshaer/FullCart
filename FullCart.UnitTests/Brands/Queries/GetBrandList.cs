using Application.Brands.Queries;
using Application.Common.Interfaces.Data;
using AutoFixture.Xunit2;
using Domain.Brands;
using FluentAssertions;
using FullCart.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FullCart.UnitTests.Brands.Queries;

public class GetBrandList
{
    
    [Theory, AutoCustomData]
    public async Task GetBrandList_ReturnsCorrectBrands(
        [Frozen] ICartDbContext dbContext,
        GetBrandsListQueryHandler handler,
        GetBrandsListQuery query,
        DbSet<Brand> brands)
    {
        // Arrange
        dbContext.Brands.Returns(brands);
        // Act
        var result= await handler.Handle(query, CancellationToken.None);
        // Assert
        result.Count.Should().Be(brands.Count());
        var expectedResult = brands.Select(GetBrandsListResponse.MapTo()).ToList();
        result.Should().BeEquivalentTo(expectedResult, options => options.ComparingByMembers<Brand>()); 
    }
}