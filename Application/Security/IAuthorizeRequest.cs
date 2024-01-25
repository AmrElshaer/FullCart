using MediatR;

namespace Application.Security;

public interface IAuthorizeRequest<T>: IRequest<T>
{
    Guid UserId { get; }
}
