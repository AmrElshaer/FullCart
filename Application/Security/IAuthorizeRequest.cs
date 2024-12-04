using MediatR;

namespace Application.Security;

public interface IAuthorizeRequest<T>: IRequest<T>
{
}

public interface IAuthorizeddCommand<T> : IAuthorizeRequest<T>
{
    
}
