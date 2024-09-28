using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Data;
using Application.Common.Interfaces.File;
using Domain.Categories;
using ErrorOr;
using MediatR;

namespace Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ErrorOr<Guid>>
{
    private readonly ICartDbContext _dbContext;
    private readonly IFileAppService _fileAppService;

    public UpdateCategoryCommandHandler(ICartDbContext dbContext, IFileAppService fileAppService)
    {
        _dbContext = dbContext;
        _fileAppService = fileAppService;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.Id);

        if (category is null) return Error.NotFound("Category not found");
        var categoryName = CategoryName.Create(request.Name);

        if (categoryName.IsError) return categoryName.Errors;

        var fileName = await _fileAppService.UploadFileAsync(FileType.Image, request.Image);
        await _fileAppService.DeleteFileAsync(FileType.Image, category.FileName.FileName);
        var categoryFileName = CategoryFileName.Create(fileName);

        if (categoryName.IsError) return categoryFileName.Errors;

        category.Update(categoryName.Value, categoryFileName.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}