using BuildingBlocks.Application.Common.Behaviours;
using BuildingBlocks.Application.Common.Extensions;
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


        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        // services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddScoped<IMediator, Mediator>();

        services.Scan(scan =>
        {
            scan.FromAssembliesOf(typeof(AssemblyInfo))
                .RegisterHandlers(typeof(IRequestHandler<>))
                .RegisterHandlers(typeof(IRequestHandler<,>))
                .RegisterHandlers(typeof(INotificationHandler<>));
        });

        services.Decorate(typeof(INotificationHandler<>), typeof(DomainEventsDispatcherDecorator<>));
        services.Decorate(typeof(IRequestHandler<,>), typeof(UnitOfWorkDecorator<,>));

        return services;
    }
}