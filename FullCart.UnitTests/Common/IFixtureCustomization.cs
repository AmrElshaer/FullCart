using AutoFixture;

namespace FullCart.UnitTests.Common;

public interface IFixtureCustomization
{
    void Customize(IFixture fixture);
}