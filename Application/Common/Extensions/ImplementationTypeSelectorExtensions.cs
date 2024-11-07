using Application.Common.Behaviours;
using Scrutor;

namespace Application.Common.Extensions;

public static class ImplementationTypeSelectorExtensions
{
    private static readonly HashSet<Type> _decorators;

    static ImplementationTypeSelectorExtensions()
    {
        _decorators = new HashSet<Type>(new[]
        {
            typeof(DispatchingIntegrationEventDecorator<>),
            typeof(AlternativeHandlerDecorator<,>)
        });
    }

    public static IImplementationTypeSelector RegisterHandlers(this IImplementationTypeSelector selector, Type type)
    {
        return selector.AddClasses(c =>
                c.AssignableTo(type)
                    .Where(t => !_decorators.Contains(t))
            )
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .AsImplementedInterfaces()
            .WithScopedLifetime();
    }
    public static IImplementationTypeSelector RegisterMediatorHandler(this IImplementationTypeSelector selector,Type type)
    {
        return selector.AddClasses(classes => classes
                .AssignableTo(type)
                .Where(type =>
                    !_decorators.Contains(type) &&
                    !type.GetInterfaces()
                        .Any(i => i.IsGenericType &&
                                 i.GetGenericTypeDefinition() == typeof(IAlternativeHandler<,>))))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .AsImplementedInterfaces()
            .WithScopedLifetime();
    }
}
