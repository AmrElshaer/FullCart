using System.Text;
using System.Threading.RateLimiting;
using Application;
using Application.Common.Interfaces;
using Application.Common.Interfaces.File;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BuildingBlocks.Application.Common.Interfaces.Authentication;
using BuildingBlocks.Infrastucture.Common.CurrentUserProvider;
using FullCart.API.Modules.Order;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Persistence;
using Infrastructure.Hubs.OrderHub;
using Infrastructure.Security.TokenGenerator;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddRateLimiter(l =>
{
    l.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    l.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Define the security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.TryAddScoped<ICurrentUserProvider, CurrentUserProvider>();
var jwtSettings = builder.Configuration.GetSection(JwtSettings.Section).Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidIssuer = jwtSettings.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IFileAppService, FileAppService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Section));

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddInfrastructure(builder.Configuration)
    .AddApplication();
// Use Autofac as the DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Configure Autofac container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new OrderAutoFacModule());
});
var app = builder.Build();
// var container = app.Services.GetAutofacRoot();
InitializeModules(app.Configuration);
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.InitialiseDatabase();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.MapHub<OrderStatusHub>("/orderStatusHub");
app.Run();

void InitializeModules(IConfiguration appConfiguration)
{
    OrdersStartup.Initialize(appConfiguration);
}