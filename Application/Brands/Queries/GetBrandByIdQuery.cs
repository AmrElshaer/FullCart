using System.Linq.Expressions;
using BuildingBlocks.Application.Security;
using Domain.Brands;
using Microsoft.EntityFrameworkCore;

namespace Application.Brands.Queries;

[Authorize(Roles = Roles.Admin)]
public record GetBrandByIdQuery(Guid Id) : IRequest<GetBrandByIdResponse>;

public readonly record struct GetBrandByIdResponse(Guid Id, string Name, string FileName)
{
    public static Expression<Func<Brand, GetBrandByIdResponse>> MapTo()
    {
        return c => new GetBrandByIdResponse(c.Id, c.Name.Name, c.FileName.FileName);
    }
}

internal class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, GetBrandByIdResponse>
{
    private readonly ICartDbContext _cartDbContext;

    public GetBrandByIdQueryHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task<GetBrandByIdResponse> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        return await _cartDbContext.Brands
            .Where(c => c.Id == request.Id)
            .Select(GetBrandByIdResponse.MapTo())
            .FirstOrDefaultAsync(cancellationToken);
    }
}