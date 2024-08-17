using Application.Common.Behaviours;
using Application.Common.Extensions;
using Application.Orders.Commands.CreateOrder;
using Domain.Common;
using Domain.Orders.Events;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // services.AddMediatR(options =>
        // {
        //    // options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        //
        //     options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        //     options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        // });
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
       // services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddScoped<IMediator, Mediator>();

        services.Scan(scan =>
        {
            scan.FromAssembliesOf(typeof(OrderPlacedDomainEventHandler))
                .RegisterHandlers(typeof(IRequestHandler<>))
                .RegisterHandlers(typeof(IRequestHandler<,>))
                .RegisterHandlers(typeof(INotificationHandler<>));
        });

        services.Decorate(typeof(INotificationHandler<>), typeof(DispatchingIntegrationEventDecorator<>));

        return services;
    }

    private static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

            if (generic == cur)
            {
                return true;
            }

            toCheck = toCheck.BaseType;
        }

        return false;
    }
}
