using System.Reflection;
using System.Text;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Hubs;
using Application.Orders.Commands.CreateOrder;
using Domain.Payments.Events;
using Domain.Roles;
using Domain.Users;
using DotNetCore.CAP;
using EFCore.AuditExtensions.SqlServer;
using Infrastructure.BackgroundJobs;
using Infrastructure.Common;
using Infrastructure.Common.Persistence;
using Infrastructure.Hubs.OrderHub;
using Infrastructure.Payments.Consumer;
using Infrastructure.Security;
using Infrastructure.Security.CurrentUserProvider;
using Infrastructure.Security.TokenGenerator;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Savorboard.CAP.InMemoryMessageQueue;

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
            .AddServices(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .UseSqlServerAudit();
            //  .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());

            options.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<CartDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICartDbContext>(provider => provider.GetRequiredService<CartDbContext>());

        services.AddHostedService<ApplyMigrationHostedService>();
        var jwtSettings = configuration.GetSection(JwtSettings.Section).Get<JwtSettings>();
        ArgumentNullException.ThrowIfNull(jwtSettings);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                };
            });

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.TryAddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HostOptions>(x =>
        {
            x.ServicesStartConcurrently = true;
            x.ServicesStopConcurrently = false;
        });

       // services.AddHostedService<NumberOfOrdersJob>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IFileAppService, FileAppService>();
        services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());

            busConfigurator.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Immediate(2));
            });

            busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(host: "localhost", virtualHost: "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                busFactoryConfigurator.ReceiveEndpoint("service1-queue", e =>
                {
                    e.Bind<PaymentCreated>();
            
                    e.ConfigureConsumer<PaymentConsumer>(context);
                });
                busFactoryConfigurator.ReceiveEndpoint("service2-queue", e =>
                {
                    e.Bind<PaymentCreated>();
            
                    e.ConfigureConsumer<PaymentConsumer>(context);
                });
               // busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        services.AddCap(capOptions =>
        {
            capOptions.UseSqlServer(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.Schema = "outbox";
            });

            capOptions.UseInMemoryMessageQueue();

            capOptions.UseDashboard(dashboardOptions =>
            {
                dashboardOptions.PathMatch = "/cap";
            });
        });

        services.Scan(scan =>
        {
            scan.FromAssemblies(typeof(OrderPlacedIntegrationEventHandler).Assembly)
                .AddClasses(filter => filter.AssignableTo<ICapSubscribe>())
                .AsSelf()
                .WithTransientLifetime();
        });

        services.AddTransient<IOrderHub, OrderHub>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
