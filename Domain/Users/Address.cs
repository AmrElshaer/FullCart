using Domain.Common;
using ErrorOr;

namespace Domain.Users;

public class Address:ValueObject
{
   

    public string Street { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string State { get; private set; }= default!;

    private Address()
    {
        
    }

    private Address(string city,string street,string state)
    {
        Street = street;
        City = city;
        State = state;
    }

    public static ErrorOr<Address> Create(string city,string street,string state)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return Error.Validation("Street must have value");
        }

        return new Address(city,street,state);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
    }
}
