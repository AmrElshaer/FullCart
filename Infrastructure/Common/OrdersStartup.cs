using Application;
using Application.Common.Interfaces;
using Application.Orders.Commands.CreateOrder;
using Autofac;
using BuildingBlocks.Application.Common.Interfaces.Authentication;
using Infrastructure.Common.Persistence;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common;

public class OrdersStartup
{
    public static void Initialize(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        ConfigureCompositionRoot(configuration, httpContextAccessor,identityService);
    }

    private static void ConfigureCompositionRoot(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        var containerBuilder = new ContainerBuilder();

        // add all configuration of dependencies here using autofac
        var mediatRConfiguration = MediatRConfigurationBuilder
            .Create(typeof(CreateOrder).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Scoped)
            .Build();
        containerBuilder.RegisterMediatR(mediatRConfiguration);
        containerBuilder.RegisterModule(new ApplicationModule());
        containerBuilder.RegisterModule(new InfrastructureModule(configuration, httpContextAccessor,identityService));


        var container = containerBuilder.Build();
        container.InitialiseDatabase<CartDbContext>();
        OrderCompositionRoot.SetContainer(container);
    }
}