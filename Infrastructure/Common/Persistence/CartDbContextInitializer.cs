﻿using Domain.Roles;
using Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Common.Persistence;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<CartDbContextInitializer>();

        await initializer.InitialiseAsync();

        await initializer.SeedAsync();
    }
}

public class CartDbContextInitializer
{
    private readonly ILogger<CartDbContextInitializer> _logger;
    private readonly CartDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public CartDbContextInitializer(ILogger<CartDbContextInitializer> logger, CartDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);

            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);

            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        var rolesToAdd = Roles.GetRoles()
            .Except(await _roleManager.Roles.Select(r => r.Name).ToListAsync())
            .ToList();

        foreach (var role in rolesToAdd)
            await _roleManager.CreateAsync(new Role(role));

        var administratorRole = new Role(Roles.Admin);
        var adminEmail = Email.Create("administrator@localhost.com");
        var adminId = Guid.NewGuid();
        var administrator = User.Create(adminId, adminEmail.Value, UserType.Admin);

        if (_userManager.Users.All(u => u.UserName != administrator.Value.UserName))
        {
            await _userManager.CreateAsync(administrator.Value, password: "Administrator1!");

            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                await _userManager.AddToRolesAsync(administrator.Value, new[]
                {
                    administratorRole.Name,
                });
        }

        // Default data
        // Seed, if necessary
        if (!_context.Admins.Any(a => a.User.UserName == administrator.Value.UserName))
        {
            _context.Admins.Add(new Admin(adminId));

            await _context.SaveChangesAsync();
        }
    }
}

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Error happen `{Ex}`")]
    public static partial void LogException(this ILogger logger, Exception ex);
}
