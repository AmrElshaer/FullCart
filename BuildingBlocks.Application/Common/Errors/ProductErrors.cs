using ErrorOr;

namespace BuildingBlocks.Application.Common.Errors;

public static class ProductErrors
{
    public static Func<IReadOnlyList<Guid>,Error> NotFoundProducts =notFoundProductsIds=> Error.Validation(
        code: "Products.NotFound",
        description: $"Can't find products that have this ids {string.Join(',',notFoundProductsIds)}");
}
