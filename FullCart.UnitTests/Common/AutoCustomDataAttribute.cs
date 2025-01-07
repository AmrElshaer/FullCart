using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;

namespace FullCart.UnitTests.Common;

internal sealed class AutoCustomDataAttribute() : AutoDataAttribute(() =>
{
    var fixture = new Fixture();
    var customizations = Assembly.GetExecutingAssembly().GetTypes()
        .Where(type => typeof(IFixtureCustomization).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
        .Select(type => Activator.CreateInstance(type) as IFixtureCustomization)
        .ToList();
    foreach (var customization in customizations) customization?.Customize(fixture);
    return fixture;
});