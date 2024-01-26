using Application.Common.Enums;
using Application.Common.Interfaces;
using Domain.Brands;
using ErrorOr;
using MediatR;

namespace Application.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler:IRequestHandler<CreateBrandCommand,ErrorOr<Guid>> {
    private readonly ICartDbContext _dbContext;
    private readonly IFileAppService _fileAppService;

    public CreateBrandCommandHandler(ICartDbContext dbContext,IFileAppService fileAppService)
    {
        _dbContext = dbContext;
        _fileAppService = fileAppService;
    }
    public async Task<ErrorOr<Guid>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brandName = BrandName.Create(request.Name);

        if (brandName.IsError)
        {
            return brandName.Errors;
        }

        var fileName = await _fileAppService.UploadFileAsync(FileType.Image, request.Image);
        var brandFileName = BrandFileName.Create(fileName);

        if (brandName.IsError)
        {
            return brandFileName.Errors;
        }

        var brand = new Brand(Guid.NewGuid(),
            brandName.Value,
            brandFileName.Value);

        await _dbContext.Brands.AddAsync(brand,cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}
