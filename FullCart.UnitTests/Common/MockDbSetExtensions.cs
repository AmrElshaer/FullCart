using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FullCart.UnitTests.Common;

public static class MockDbSetExtensions
{
    public static DbSet<T> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
    {
        var mockSet = Substitute.For<DbSet<T>, IQueryable<T>, IAsyncEnumerable<T>>();

        var queryProvider = new TestAsyncQueryProvider<T>(data.Provider);

        mockSet.As<IQueryable<T>>().Provider.Returns(queryProvider);
        mockSet.As<IQueryable<T>>().Expression.Returns(data.Expression);
        mockSet.As<IQueryable<T>>().ElementType.Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().GetEnumerator().Returns(data.GetEnumerator());
        mockSet.As<IAsyncEnumerable<T>>().GetAsyncEnumerator(default)
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

        return mockSet;
    }
}