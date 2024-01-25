using Domain.Common;
using ErrorOr;

namespace Domain.Users;

public class Customer:Entity
{
    public User User { get; } = null!;

    public Address? Address { get; private set; }

    private Customer(Guid id,Address? address)
    {
        Id = id;
        Address = address;
    }
}
