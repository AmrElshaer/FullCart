using Microsoft.AspNetCore.Identity;

namespace Domain.Roles;

public class Role: IdentityRole<Guid>
{
    private Role()
        : base()
    {
        
    }
    public Role(string roleName) : base(roleName)
    {
        
    }
}
