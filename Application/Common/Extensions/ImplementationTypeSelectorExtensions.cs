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
            typeof(DispatchingIntegrationEventDecorator<>)
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
}
