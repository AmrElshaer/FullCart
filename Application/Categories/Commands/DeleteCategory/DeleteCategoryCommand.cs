using BuildingBlocks.Application.Security;

namespace Application.Categories.Commands.DeleteCategory;

[Authorize(Roles = Roles.Admin)]
public record DeleteCategoryCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ErrorOr<Success>>
{
    private readonly ICartDbContext _cartDbContext;

    public DeleteCategoryCommandHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _cartDbContext.Categories.FindAsync(request.Id);

        if (category is null) return Error.NotFound("Category not found");

        _cartDbContext.Categories.Remove(category);
        await _cartDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}