using BuildingBlocks.Application.Common.Interfaces.Data;
using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Roles.Persistence;

// public class RoleDataSeeder(RoleManager<Role> roleManager) : IDataSeeder
// {
//     public async Task SeedAllAsync()
//     {
//         var rolesToAdd = Domain.Roles.Roles.GetRoles()
//             .Except(await roleManager.Roles.Select(r => r.Name).ToListAsync())
//             .ToList();
//
//         foreach (var role in rolesToAdd)
//             await roleManager.CreateAsync(new Role(role));
//     }
// }