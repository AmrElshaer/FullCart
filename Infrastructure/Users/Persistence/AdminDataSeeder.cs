namespace Infrastructure.Users.Persistence;

// public class AdminDataSeeder(ICartDbContext cartDbContext, UserManager<User> userManager) : IDataSeeder
// {
//     public IEnumerable<Type> GetDependentDataSeeders()
//     {
//         return
//         [
//             typeof(RoleDataSeeder)
//         ];
//     }
//
//     public async Task SeedAllAsync()
//     {
//         var administratorRole = new Role(Domain.Roles.Roles.Admin);
//         var adminEmail = Email.Create("administrator@localhost.com");
//         var adminId = Guid.NewGuid();
//         var administrator = User.Create(adminId, adminEmail.Value, UserType.Admin);
//
//         if (userManager.Users.All(u => u.UserName != administrator.Value.UserName))
//         {
//             await userManager.CreateAsync(administrator.Value, "Administrator1!");
//
//             if (!string.IsNullOrWhiteSpace(administratorRole.Name))
//                 await userManager.AddToRolesAsync(administrator.Value, new[]
//                 {
//                     administratorRole.Name
//                 });
//         }
//
//
//         if (!cartDbContext.Admins.Any(a => a.User.UserName == administrator.Value.UserName))
//         {
//             cartDbContext.Admins.Add(new Admin(adminId));
//
//             await cartDbContext.SaveChangesAsync(default);
//         }
//     }
// }