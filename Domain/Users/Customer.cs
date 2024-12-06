using BuildingBlocks.Domain.Common;
namespace Domain.Users;

public class Customer:Entity
{
    public User User { get; } = null!;

    public Address? Address { get; private set; }

    private Customer()
    {
        
    }

    public Customer(Guid id,Address address)
    {
        Id = id;
        Address = address;
    }
}
