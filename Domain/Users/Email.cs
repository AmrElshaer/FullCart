using System.Text.RegularExpressions;
using Domain.Common;
using ErrorOr;

namespace Domain.Users;

public class Email:ValueObject
{
    public string Value { get; private set; }= default!;

    private Email()
    {
        
    }

    private Email(string value) { Value = value; }

    public static ErrorOr<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Error.Validation("Email must have value");
        }

        if (!IsValidEmail(email))
        {
            return Error.Validation("Not Valid Email");
        }
        return new Email(email);
    }
    private static bool IsValidEmail(string email)
    {
        
        var pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        var regex = new Regex(pattern);
        return regex.IsMatch(email);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
