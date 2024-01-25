using Domain.Common;

namespace Domain.Users;

public class Admin:Entity
{
    
    public bool IsCentralized { get; private set; }

    public User User { get; } = null!;

    public Admin(Guid id, bool isCentralized=true)
    {
        IsCentralized = isCentralized;
        Id = id;
    }
}
