using System.Reflection;
using Application.Common.Interfaces;
using Application.Orders.Commands.CreateOrder;
using Domain.Roles;
using Domain.Users;
using DotNetCore.CAP;
using Infrastructure.Common;
using Infrastructure.Common.Persistence;
using Infrastructure.Outbox;
using Infrastructure.Security;
using Infrastructure.Security.CurrentUserProvider;
using Infrastructure.Security.TokenGenerator;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        //services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddDbContext<CartDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            //  .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());

            options.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        services.AddScoped<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

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

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxJob));

            configure
                .AddJob<ProcessOutboxJob>(jobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
                        schedule => schedule.WithIntervalInSeconds(1).RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
     
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IFileAppService, FileAppService>();
        services.AddTransient<IEventDispatcher, EventDispatcher>();

        services.Scan(scan =>
        {
            scan.FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IJob)))
                .AsSelf()
                .WithTransientLifetime();
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

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
