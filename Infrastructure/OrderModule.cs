using Application.Common.Interfaces;
using Infrastructure.Common;
using MediatR;

namespace Infrastructure;

public class OrderModule : IOrderModule
{
    public async Task<TResult> SendAsync<TResult>(IRequest<TResult> command)
    {
        return await CommandsExecutor.SendAsync(command);
    }

    public async Task SendAsync(IRequest command)
    {
        await CommandsExecutor.SendAsync(command);
    }
}