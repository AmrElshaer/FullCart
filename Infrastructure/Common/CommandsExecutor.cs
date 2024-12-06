using Autofac;
using MediatR;

namespace Infrastructure.Common;

internal static class CommandsExecutor
{
    internal static async Task SendAsync(IRequest command)
    {
        using (var scope = OrderCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            await mediator.Send(command);
        }
    }

    internal static async Task<TResult> SendAsync<TResult>(IRequest<TResult> command)
    {
        using (var scope = OrderCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            return await mediator.Send(command);
        }
    }
}