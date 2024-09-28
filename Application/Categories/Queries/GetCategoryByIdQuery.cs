using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Data;
using Application.Security;
using Domain.Categories;
using Domain.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Queries;

[Authorize(Roles = Roles.Admin)]
public record GetCategoryByIdQuery(Guid Id) : IRequest<GetCategoryByIdResponse>;

public readonly record struct GetCategoryByIdResponse(Guid Id, string Name, string FileName)
{
    public static Expression<Func<Category, GetCategoryByIdResponse>> MapTo()
    {
        return c => new GetCategoryByIdResponse(c.Id, c.Name.Name, c.FileName.FileName);
    }
}

internal class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, GetCategoryByIdResponse>
{
    private readonly ICartDbContext _cartDbContext;

    public GetCategoryByIdQueryHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task<GetCategoryByIdResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _cartDbContext.Categories
            .Where(c => c.Id == request.Id)
            .Select(GetCategoryByIdResponse.MapTo())
            .FirstOrDefaultAsync(cancellationToken);
    }
}