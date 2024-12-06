using System.Linq.Expressions;
using BuildingBlocks.Application.Security;
using Domain.Brands;
using Microsoft.EntityFrameworkCore;

namespace Application.Brands.Queries;

[Authorize(Roles = Roles.Admin)]
public record GetBrandsListQuery() : IRequest<IList<GetBrandsListResponse>>;

public readonly record struct GetBrandsListResponse(Guid Id, string Name, string FileName)
{
    public static Expression<Func<Brand, GetBrandsListResponse>> MapTo()
    {
        return c => new GetBrandsListResponse(c.Id, c.Name.Name, c.FileName.FileName);
    }
}

public class GetBrandsListQueryHandler : IRequestHandler<GetBrandsListQuery, IList<GetBrandsListResponse>>
{
    private readonly ICartDbContext _cartDbContext;

    public GetBrandsListQueryHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task<IList<GetBrandsListResponse>> Handle(GetBrandsListQuery request,
        CancellationToken cancellationToken)
    {
        return await _cartDbContext.Brands.Select(GetBrandsListResponse.MapTo()).ToListAsync(cancellationToken);
    }
}