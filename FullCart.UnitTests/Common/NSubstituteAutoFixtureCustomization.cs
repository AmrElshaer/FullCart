using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace FullCart.UnitTests.Common;

public class NSubstituteAutoFixtureCustomization : IFixtureCustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize(new AutoNSubstituteCustomization());
    }
}