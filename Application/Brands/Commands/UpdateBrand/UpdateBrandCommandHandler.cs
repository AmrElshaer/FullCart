using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Data;
using Application.Common.Interfaces.File;
using Domain.Brands;
using ErrorOr;
using MediatR;

namespace Application.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, ErrorOr<Guid>>
{
    private readonly ICartDbContext _dbContext;
    private readonly IFileAppService _fileAppService;

    public UpdateBrandCommandHandler(ICartDbContext dbContext, IFileAppService fileAppService)
    {
        _dbContext = dbContext;
        _fileAppService = fileAppService;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _dbContext.Brands.FindAsync(request.Id);

        if (brand is null) return Error.NotFound("Brand not found");
        var brandName = BrandName.Create(request.Name);

        if (brandName.IsError) return brandName.Errors;

        var fileName = await _fileAppService.UploadFileAsync(FileType.Image, request.Image);
        await _fileAppService.DeleteFileAsync(FileType.Image, brand.FileName.FileName);
        var brandFileName = BrandFileName.Create(fileName);

        if (brandName.IsError) return brandFileName.Errors;

        brand.Update(brandName.Value, brandFileName.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}