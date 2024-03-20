namespace Domain.Roles;

public abstract class Roles
{
    public const string Admin = nameof(Admin);

    public const string Customer = nameof(Customer);

    public static IEnumerable<string> GetRoles()
    {
        yield return Admin;
        yield return Customer;
    }
}
