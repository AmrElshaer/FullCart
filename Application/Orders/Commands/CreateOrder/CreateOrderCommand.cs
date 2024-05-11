using Application.Security;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrder
{
    public record CreateOrderItemRequest(Guid ProductId, int Quantity);

    [Authorize(Roles = Roles.Customer)]
    public record Command(IReadOnlyList<CreateOrderItemRequest> Items) : IAuthorizeRequest<ErrorOr<Guid>>;

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(r => r.Items)
                .NotEmpty()
                .NotNull();

            RuleFor(r => r.Items)
                .Must(items => items.Select(item => item.ProductId).Distinct().Count() == items.Count)
                .WithMessage("Duplicate products are not allowed.");
        }
    }

    internal class Handler : IRequestHandler<Command, ErrorOr<Guid>>
    {
        private readonly ICartDbContext _db;
        private readonly ICurrentUserProvider _currentUserProvider;

        public Handler(ICartDbContext db, ICurrentUserProvider currentUserProvider)
        {
            _db = db;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<ErrorOr<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var orderItems = await CreateOrderItems(request.Items, cancellationToken);

            if (orderItems.IsError)
                return orderItems.Errors;

            var user = _currentUserProvider.GetCurrentUser();
            var userId = user.Id;
            var order = new Order(Guid.NewGuid(), userId, orderItems.Value);
            await _db.Orders.AddAsync(order, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return order.Id;
        }

        private async Task<ErrorOr<IReadOnlyList<OrderItem>>> CreateOrderItems(IReadOnlyList<CreateOrderItemRequest> items, CancellationToken cancellationToken)
        {
            var productsIds = items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            var products = await _db.Products.Where(p =>
                    productsIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            var notFoundProducts = productsIds.Except(products.Keys)
                .ToList();

            if (notFoundProducts.Any())
                return ProductErrors.NotFoundProducts(notFoundProducts);

            IReadOnlyList<ErrorOr<OrderItem>> orderItemsOrErrors = items
                .Select(CreateOrderItem(products)).ToList();

            var ordersItemsHaveErrors = orderItemsOrErrors.Where(i => i.IsError)
                .ToList();

            if (ordersItemsHaveErrors.Any())
                return ordersItemsHaveErrors.SelectMany(i => i.Errors).ToList();

            return orderItemsOrErrors.Select(oi => oi.Value)
                .ToList();
        }

        private static Func<CreateOrderItemRequest, ErrorOr<OrderItem>> CreateOrderItem(Dictionary<Guid, Product> products)
        {
            return i =>
            {
                var quantity = OrderItemQuantity.Create(i.Quantity);

                if (quantity.IsError)
                    return quantity.Errors;

                var product = products[i.ProductId];

                return new OrderItem(product.Id, quantity.Value, product.Price);
            };
        }
    }
}
