namespace Application.Common.Interfaces;

public interface IOrderModule
{
    Task<TResult> SendAsync<TResult>(IRequest<TResult> command);

    Task SendAsync(IRequest command);
}