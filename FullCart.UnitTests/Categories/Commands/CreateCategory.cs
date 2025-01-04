using Application.Categories.Commands.CreateCategory;
using Application.Common.Enums;
using Application.Common.Interfaces.Data;
using Application.Common.Interfaces.File;
using Domain.Categories;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace FullCart.UnitTests.Categories.Commands;

public class CreateCategory
{
    private readonly ICartDbContext _dbContext = Substitute.For<ICartDbContext>();
    private readonly IFileAppService _fileAppService = Substitute.For<IFileAppService>();

    [Fact]
    public async Task CreateCategoryCommand_EmptyCategoryName_ReturnInvalidResult()
    {
        // Arrange
        var command = new CreateCategoryCommand("", Substitute.For<IFormFile>());
        var handler = new CreateCategoryCommandHandler(_dbContext,_fileAppService);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation("Category must have file name"));
       
    }
    [Fact]
    public async Task CreateCategoryCommand_NullCategoryName_ReturnInvalidResult()
    {
        // Arrange
        var command = new CreateCategoryCommand(null, Substitute.For<IFormFile>());
        var handler = new CreateCategoryCommandHandler(_dbContext,_fileAppService);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation("Category must have file name"));
       
    }
    [Fact]
    public async Task CreateCategoryCommand_NullImage_ReturnInvalidResult()
    {
        // Arrange
        var command = new CreateCategoryCommand("TestCategory", null);
        var handler = new CreateCategoryCommandHandler(_dbContext,_fileAppService);
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation("Category must have name"));
       
    }
    [Fact]
    public async Task CreateCategoryCommand_ValidInput_ReturnGuid()
    {
        // Arrange
        var categoryName = CategoryName.Create("TestCategory");
        var fileName = CategoryFileName.Create("TestFileName");
        var command = new CreateCategoryCommand("TestCategory", Substitute.For<IFormFile>());
        var handler = new CreateCategoryCommandHandler(_dbContext,_fileAppService);
        _fileAppService.UploadFileAsync(FileType.Image, command.Image).Returns("TestFileName");
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        
        await _dbContext.Received(1).Categories.AddAsync(
            Arg.Is<Category>(c => c.Name == categoryName   && c.FileName ==  fileName && c.Id==result.Value),
            Arg.Any<CancellationToken>());

        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    }
}