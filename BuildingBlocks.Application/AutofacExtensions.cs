using System.Reflection;
using Autofac;
using BuildingBlocks.Application.Common.Behaviours;

namespace BuildingBlocks.Application;

public static class AutofacExtensions
{
    private static readonly HashSet<Type> _decorators = new()
    {
        typeof(UnitOfWorkDecorator<,>),
        typeof(DomainEventsDispatcherDecorator<>)
    };

    public static void RegisterHandlers(this ContainerBuilder builder, Type handlerType, Assembly assembly)
    {
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => handlerType.IsAssignableFrom(t) && !_decorators.Contains(t))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}