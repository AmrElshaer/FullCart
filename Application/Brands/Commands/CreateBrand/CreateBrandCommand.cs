using BuildingBlocks.Application.Security;
using Microsoft.AspNetCore.Http;

namespace Application.Brands.Commands.CreateBrand;

[Authorize(Roles = Roles.Admin)]
public record CreateBrandCommand(string Name, IFormFile Image) : IRequest<ErrorOr<Guid>>;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Image)
            .Must(BeAValidImage).WithMessage("Image is required and must be a valid file.");
    }

    private static bool BeAValidImage(IFormFile? image)
    {
        if (image == null)
            return false;

        // Check if the file is an image by its content type
        if (image.ContentType.StartsWith("image/"))
        {
            // Get the file extension
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

            // Check if the file extension is png or jpg/jpeg
            return fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg";
        }

        return false;
    }
}