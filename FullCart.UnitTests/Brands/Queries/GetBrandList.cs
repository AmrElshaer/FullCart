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
        ICartDbContext dbContext,
        GetBrandsListQueryHandler handler,
        GetBrandsListQuery query,
        [Greedy] List<Brand> brands)
    {
       
        dbContext.Brands.ToListAsync().Returns(Task.FromResult(brands));
        var result= await handler.Handle(query, CancellationToken.None);
        result.Should().BeEquivalentTo(brands);
        
    }
}