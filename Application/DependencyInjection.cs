using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Extensions;
using Application.Orders.Commands.CreateOrder;
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
}
