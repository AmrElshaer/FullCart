using Application.Common.Interfaces.Data;
using Domain.Comments;
using Domain.Orders;

namespace Application.Comments.Commands.CreateComment;

public record CreateCommentCommand(Guid OrderId, string Content) : IRequest<Guid>;

internal record CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICartDbContext _cartDbContext;

    public CreateCommentCommandHandler(ICartDbContext cartDbContext)
    {
        _cartDbContext = cartDbContext;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = Comment.Create(OrderId.Create(request.OrderId), request.Content);
        await _cartDbContext.Comments.AddAsync(comment, cancellationToken);
        await _cartDbContext.SaveChangesAsync(cancellationToken);
        return comment.Id.Value;
    }
}