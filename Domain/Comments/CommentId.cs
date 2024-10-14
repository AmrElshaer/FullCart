using Domain.Common;

namespace Domain.Comments;

public class CommentId : ValueObject
{
    public Guid Value { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static CommentId CreateUniqueId()
    {
        return Create(
            Guid.NewGuid()
        );
    }

    public static CommentId Create(Guid value)
    {
        return new CommentId()
        {
            Value = value
        };
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}