using Application.Common.Interfaces.Data;
using Domain.Roles;
using Domain.Users;
using Infrastructure.Roles.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Users.Persistence;

public class CustomerDataSeeder(ICartDbContext cartDbContext, UserManager<User> userManager) : IDataSeeder
{
    public IEnumerable<Type> GetDependentDataSeeders()
    {
        return
        [
            typeof(RoleDataSeeder)
        ];
    }

    public async Task SeedAllAsync()
    {
        var customerRole = new Role(Domain.Roles.Roles.Customer);
        var customerEmail = Email.Create("customer@localhost.com");
        var customerId = Guid.NewGuid();
        var customer = User.Create(customerId, customerEmail.Value, UserType.Customer).Value;

        if (userManager.Users.All(u => u.UserName != customer.UserName))
        {
            await userManager.CreateAsync(customer, "Customer@1!");

            if (!string.IsNullOrWhiteSpace(customerRole.Name))
                await userManager.AddToRolesAsync(customer, new[]
                {
                    customerRole.Name
                });
        }


        if (!cartDbContext.Customers.Any(a => a.User.UserName == customer.UserName))
        {
            var customerAddress = Address.Create("customer", "123 Main Street", "customer").Value;
            cartDbContext.Customers.Add(new Customer(customerId, customerAddress));

            await cartDbContext.SaveChangesAsync(default);
        }
    }
}