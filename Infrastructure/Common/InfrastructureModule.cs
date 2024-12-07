using System.Reflection;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Event;
using Application.Common.Interfaces.File;
using Application.Common.Interfaces.Hubs;
using Application.Orders.Commands.CreateOrder;
using Autofac;
using BuildingBlocks.Application.Common.Interfaces;
using BuildingBlocks.Application.Common.Interfaces.Authentication;
using BuildingBlocks.Application.Common.Interfaces.Data;
using BuildingBlocks.Infrastucture.Common;
using BuildingBlocks.Infrastucture.Common.CurrentUserProvider;
using BuildingBlocks.Infrastucture.Common.Persistence.Seeder;
using Domain.Roles;
using Domain.Users;
using DotNetCore.CAP;
using EFCore.AuditExtensions.SqlServer;
using Infrastructure.Common.Persistence;
using Infrastructure.Hubs.OrderHub;
using Infrastructure.Security.TokenGenerator;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Savorboard.CAP.InMemoryMessageQueue;
using Module = Autofac.Module;

namespace Infrastructure.Common;

public class InfrastructureModule : Module
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public InfrastructureModule(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    protected override void Load(ContainerBuilder builder)
    {
        // Add all dependency registrations here using Autofac

       
        builder.Register(context =>
        {
            var jwtSettings = _configuration.GetSection(JwtSettings.Section).Get<JwtSettings>();
            return Options.Create(jwtSettings!);
        }).As<IOptions<JwtSettings>>().SingleInstance();
        builder.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().SingleInstance();
    
        builder.RegisterType<FileAppService>().As<IFileAppService>().InstancePerDependency();
        builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
        builder.RegisterType<CurrentUserProvider>().As<ICurrentUserProvider>().InstancePerLifetimeScope();
        builder.RegisterType<CurrentUserProvider>()
            .As<ICurrentUserProvider>() // Register for the ICurrentUserProvider interface
            .AsSelf() // Register for the concrete type (optional, if needed elsewhere)
            .WithParameter(
                (parameter, context) => parameter.ParameterType == typeof(IHttpContextAccessor),
                (parameter, context) => _httpContextAccessor
            )
            .InstancePerLifetimeScope();
        builder.RegisterInstance(_identityService);
        // Register DbContext and related services
        builder.Register(ctx =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<CartDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                .UseSqlServerAudit()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
            return new CartDbContext(optionsBuilder.Options);
        }).AsSelf().As<ICartDbContext>().InstancePerLifetimeScope();
        builder.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().SingleInstance();

        // Add Identity
        builder.RegisterType<UserManager<User>>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterType<SignInManager<User>>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterType<RoleManager<Role>>().AsSelf().InstancePerLifetimeScope();

        // CAP Configuration
        builder.Register(context =>
        {
            var capOptions = new CapOptions();
            capOptions.UseSqlServer(options =>
            {
                options.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
                options.Schema = "outbox";
            });
            capOptions.UseInMemoryMessageQueue();
            capOptions.UseDashboard(options => options.PathMatch = "/cap");
            return capOptions;
        }).AsSelf().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(OrderPlacedIntegrationEventHandler).Assembly)
            .AssignableTo<ICapSubscribe>()
            .AsSelf()
            .InstancePerDependency();
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .AssignableTo<IDataSeeder>()
            .As<IDataSeeder>()
            .InstancePerLifetimeScope();
        builder.RegisterType<SeederExecutor>().InstancePerLifetimeScope();

        // Register Domain Event Dispatcher and Unit of Work
        builder.RegisterType<DomainEventsAccessor>().As<IDomainEventsAccessor>();
        builder.RegisterType<DomainEventDispatcher>().As<IDomainEventDispatcher>().InstancePerLifetimeScope();
        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        // hub
        builder.RegisterType<OrderHub>().As<IOrderHub>().InstancePerDependency();
    }
}