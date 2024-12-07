using Autofac;
using Autofac.Features.Variance;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Common.Behaviours;

namespace Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(TimeProvider.System);
        // Register all pipeline behaviors
        builder.RegisterGeneric(typeof(AuthorizationBehavior<,>)).As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();

        // Add Validators
        builder.RegisterAssemblyTypes(typeof(ApplicationModule).Assembly)
            .AsClosedTypesOf(typeof(IValidator<>))
            .InstancePerLifetimeScope();

        // Register Request Handlers
        builder.RegisterHandlers(typeof(IRequestHandler<,>), typeof(AssemblyInfo).Assembly);
        builder.RegisterHandlers(typeof(IRequestHandler<>), typeof(AssemblyInfo).Assembly);
        builder.RegisterHandlers(typeof(INotificationHandler<>), typeof(AssemblyInfo).Assembly);


        // Register Decorators
        builder.RegisterGenericDecorator(
            typeof(DomainEventsDispatcherDecorator<>),
            typeof(INotificationHandler<>));

        builder.RegisterGenericDecorator(
            typeof(UnitOfWorkDecorator<,>),
            typeof(IRequestHandler<,>));
    }
}