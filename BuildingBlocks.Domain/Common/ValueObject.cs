namespace BuildingBlocks.Domain.Common;

public abstract class ValueObject : IComparable<ValueObject>
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null) return false;

        return left?.Equals(right!) != false;
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public int CompareTo(ValueObject? other)
    {
        if (other == null) return 1;

        // Compare equality components in a sequential manner
        using var thisComponents = GetEqualityComponents().GetEnumerator();
        using var otherComponents = other.GetEqualityComponents().GetEnumerator();

        while (thisComponents.MoveNext() && otherComponents.MoveNext())
        {
            var thisComponent = thisComponents.Current;
            var otherComponent = otherComponents.Current;

            // If components are not IComparable, throw an exception
            if (thisComponent is IComparable thisComparable && otherComponent is IComparable otherComparable)
            {
                var comparison = thisComparable.CompareTo(otherComparable);
                if (comparison != 0) return comparison; // If any component is different, return comparison result
            }
            else
            {
                throw new InvalidOperationException("One or more equality components do not implement IComparable.");
            }
        }

        // If we reach here, it means all compared components are equal
        return 0;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }


    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }
}