using Application.Security;
using Domain.Roles;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Categories.Commands.CreateCategory;

[Authorize(Roles = Roles.Admin)]
public record CreateCategoryCommand(string Name,IFormFile Image):IRequest<ErrorOr<Guid>>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
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