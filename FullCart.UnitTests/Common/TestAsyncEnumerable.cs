using System.Linq.Expressions;

namespace FullCart.UnitTests.Common;

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>
{
    private readonly IEnumerable<T> _enumerable;

    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
        _enumerable = enumerable;
    }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    {
        _enumerable = this;
    }

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        return new TestAsyncEnumerator<T>(_enumerable.GetEnumerator());
    }
}