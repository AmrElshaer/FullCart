using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http;

namespace FullCart.UnitTests.Common;

internal sealed class AutoCustomDataAttribute() : AutoDataAttribute(() =>
{
    var fixture = new Fixture();

    fixture.Register<IFormFile>(() => new MockFormFile(
        fileName: "test-file.jpg",
        contentType: "image/jpeg",
        length: 1024
    ));
    fixture.Customize(new AutoNSubstituteCustomization());
    return fixture;
});