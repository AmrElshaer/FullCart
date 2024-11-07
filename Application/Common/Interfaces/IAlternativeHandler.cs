namespace Application.Common.Interfaces;

public interface IAlternativeHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
}