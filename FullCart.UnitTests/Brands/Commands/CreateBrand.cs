using Application.Brands.Commands.CreateBrand;
using Application.Common.Enums;
using Application.Common.Interfaces.Data;
using Application.Common.Interfaces.File;
using AutoFixture.Xunit2;
using Domain.Brands;
using ErrorOr;
using FluentAssertions;
using FullCart.UnitTests.Common;
using NSubstitute;

namespace FullCart.UnitTests.Brands.Commands;

public class CreateBrand
{

    [Theory,AutoCustomData]
    public async Task CreateBrandCommand_EmptyBrandName_ReturnInvalidResult(CreateBrandCommand command,CreateBrandCommandHandler handler)
    {
        // Arrange
         command =command with{Name = ""};
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation("Brand must have name"));
       
    }
    [Theory,AutoCustomData]
    public async Task CreateBrandCommand_NullBrandName_ReturnInvalidResult(CreateBrandCommand command,
        CreateBrandCommandHandler handler)
    {
        // Arrange
        command = command with { Name = null};
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation("Brand must have name"));
       
    }
    [Theory,AutoCustomData]
    public async Task CreateBrandCommand_NullImage_ReturnInvalidResult(CreateBrandCommand command,
        CreateBrandCommandHandler handler)
    {
        // Arrange
        command = command with { Image = null };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation("Brand must have file name"));
       
    }
    // use [Greedy] if have muilt constructor
    // use [Frozen] if have one instance in all constructors
    [Theory,AutoCustomData]
    public async Task CreateBrandCommand_ValidInput_ReturnGuid(CreateBrandCommand command,
        [Frozen] IFileAppService fileAppService,
        [Frozen] ICartDbContext dbContext,
        CreateBrandCommandHandler handler)
    {
        // Arrange
        var brandName = BrandName.Create(command.Name);
        var fileName = BrandFileName.Create(command.Image.FileName);
        fileAppService.UploadFileAsync(FileType.Image, command.Image)
            .Returns(Task.FromResult(command.Image.FileName));
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        
        await dbContext.Received(1).Brands.AddAsync(
            Arg.Is<Brand>(c => c.Name == brandName &&
                               c.FileName ==  fileName &&
                               c.Id==result.Value),
            Arg.Any<CancellationToken>());

        await dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    }
}