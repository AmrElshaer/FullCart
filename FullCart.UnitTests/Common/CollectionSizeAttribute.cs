using AutoFixture;
using AutoFixture.Xunit2;

namespace FullCart.UnitTests.Common;

public class CollectionSizeAttribute : AutoDataAttribute
{
    public CollectionSizeAttribute(int size) 
        : base(() =>
        {
            var fixture = new Fixture();
            
            // Set collection size for all collections
            fixture.RepeatCount = size;
            
            return fixture;
        })
    {
    }
}