using Application.Common.Interfaces;
using Application.Security;
using Domain.Roles;
using ErrorOr;
using MediatR;

namespace Application.Brands.Commands.DeleteBrand;
[Authorize(Roles = Roles.Admin)]
public record DeleteBrandCommand(Guid Id):IRequest<ErrorOr<Success>>;
public class DeleteBrandCommandHandler:IRequestHandler<DeleteBrandCommand,ErrorOr<Success>>
{
    private readonly ICartDbContext _cartDbContext;

    public DeleteBrandCommandHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }
    public async Task<ErrorOr<Success>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _cartDbContext.Brands.FindAsync(request.Id);

        if (brand is null)
        {
            return Error.NotFound("Brand not found");
        }

        _cartDbContext.Brands.Remove(brand);
        await _cartDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}