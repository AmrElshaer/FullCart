using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Domain.Users;

public class User : IdentityUser<Guid>
{
    public UserType UserType { get; private set; }

    public Admin? Admin { get; private set; }

    public Customer? Customer { get; private set; }

    private User() { }

    public static ErrorOr<User> Create(Guid id, Email email, UserType userType)
    {
        var user = new User
        {
            Id = id,
            UserName = email!.Value,
            Email = email.Value,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserType = userType,
        };

        return user;
    }
}
