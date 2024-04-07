using Application.Common.Interfaces;
using Domain.Roles;
using Domain.Users;
using Infrastructure.Common;
using Infrastructure.Common.Persistence;
using Infrastructure.Security;
using Infrastructure.Security.CurrentUserProvider;
using Infrastructure.Security.TokenGenerator;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddHttpContextAccessor()
            .AddAuthorization()
            .AddAuthentication(configuration)
            .AddPersistence(configuration)
            .AddServices();
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddDbContext<CartDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            //  .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());

            options.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<CartDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICartDbContext>(provider => provider.GetRequiredService<CartDbContext>());
        services.AddScoped<CartDbContextInitializer>();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IFileAppService, FileAppService>();
        services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();
        // services.Decorate(typeof(INotificationHandler<>), typeof(DomainEventsDispatcherNotificationHandlerDecorator<>));

        // services.Scan(scan => scan
        //      .FromAssembliesOf(typeof(PaymentCreatedNotification))
        //      .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEvent<>)))
        //      .AsImplementedInterfaces()
        //      .WithTransientLifetime());

        // services.Decorate(
        //     typeof(SaveChangeCommandHandlerDecorator<,>),
        //     typeof(ICommandHandler<,>));
        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
