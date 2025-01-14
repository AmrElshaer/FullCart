﻿using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Data;
using Application.Common.Interfaces.File;
using Domain.Categories;
using ErrorOr;
using MediatR;

namespace Application.Categories.Commands.CreateCategory;

internal class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ErrorOr<Guid>>
{
    private readonly ICartDbContext _dbContext;
    private readonly IFileAppService _fileAppService;

    public CreateCategoryCommandHandler(ICartDbContext dbContext, IFileAppService fileAppService)
    {
        _dbContext = dbContext;
        _fileAppService = fileAppService;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryName = CategoryName.Create(request.Name);

        if (categoryName.IsError) return categoryName.Errors;

        var fileName = await _fileAppService.UploadFileAsync(FileType.Image, request.Image);
        var categoryFileName = CategoryFileName.Create(fileName);

        if (categoryFileName.IsError) return categoryFileName.Errors;

        var category = new Category(Guid.NewGuid(),
            categoryName.Value,
            categoryFileName.Value);

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}