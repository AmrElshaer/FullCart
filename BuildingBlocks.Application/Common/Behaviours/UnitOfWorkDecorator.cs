using Application.Common.Interfaces;
using BuildingBlocks.Application.Security;
using MediatR;

namespace BuildingBlocks.Application.Common.Behaviours;

public class UnitOfWorkDecorator<TRequest,TResponse> : IRequestHandler<TRequest,TResponse> where TRequest : IAuthorizeRequest<TResponse>
{
    private readonly IRequestHandler<TRequest,TResponse> _inner;
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkDecorator(IRequestHandler<TRequest,TResponse> inner,
        IUnitOfWork unitOfWork)
    {
        _inner = inner;
        _unitOfWork = unitOfWork;
    }


    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var response=  await _inner.Handle(request, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return response;
    }
}