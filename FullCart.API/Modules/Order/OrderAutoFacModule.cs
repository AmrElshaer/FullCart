using Application.Common.Interfaces;
using Autofac;
using Infrastructure;

namespace FullCart.API.Modules.Order;

public class OrderAutoFacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<OrderModule>()
            .As<IOrderModule>()
            .InstancePerLifetimeScope();
    }
}