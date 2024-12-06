using Autofac;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common;

public class OrdersStartup
{
    public static void Initialize(IConfiguration configuration)
    {
        ConfigureCompositionRoot(configuration);
    }

    private static void ConfigureCompositionRoot(IConfiguration configuration)
    {
        var containerBuilder = new ContainerBuilder();

        // add all configuration of dependencies here using autofac
        containerBuilder.RegisterModule(new InfrastructureModule(configuration));

        var container = containerBuilder.Build();

        OrderCompositionRoot.SetContainer(container);
    }
}