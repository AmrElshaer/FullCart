using System.Linq.Expressions;
using BuildingBlocks.Application.Security;
using Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Queries;

[Authorize(Roles = Roles.Admin)]
public record GetCategoriesListQuery() : IRequest<IList<GetCategoriesListResponse>>;

public readonly record struct GetCategoriesListResponse(Guid Id, string Name, string FileName)
{
    public static Expression<Func<Category, GetCategoriesListResponse>> MapTo()
    {
        return c => new GetCategoriesListResponse(c.Id, c.Name.Name, c.FileName.FileName);
    }
}

public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, IList<GetCategoriesListResponse>>
{
    private readonly ICartDbContext _cartDbContext;

    public GetCategoriesListQueryHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task<IList<GetCategoriesListResponse>> Handle(GetCategoriesListQuery request,
        CancellationToken cancellationToken)
    {
        return await _cartDbContext.Categories.Select(GetCategoriesListResponse.MapTo()).ToListAsync(cancellationToken);
    }
}