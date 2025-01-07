using AutoFixture;
using Microsoft.AspNetCore.Http;

namespace FullCart.UnitTests.Common;

public class FormFileCustomization : IFixtureCustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<IFormFile>(() => new MockFormFile(
            fileName: fixture.Create<string>(),
            contentType:  fixture.Create<string>(),
            length:fixture.Create<int>()
        ));
    }
}