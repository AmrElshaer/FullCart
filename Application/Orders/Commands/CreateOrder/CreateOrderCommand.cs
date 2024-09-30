using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Data;
using Application.Security;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

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
        private readonly TimeProvider _timeProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Handler> _logger;

        public Handler(ICartDbContext db, ICurrentUserProvider currentUserProvider, TimeProvider timeProvider
            ,
            IHttpClientFactory httpClientFactory,
            ILogger<Handler> logger)
        {
            _db = db;
            _currentUserProvider = currentUserProvider;
            _timeProvider = timeProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ErrorOr<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var orderItems = await CreateOrderItems(request.Items, cancellationToken);
            if (orderItems.IsError)
                return orderItems.Errors;
            var user = _currentUserProvider.GetCurrentUser();
            var userId = user.Id;
            var order = new Order(Guid.NewGuid(), userId, orderItems.Value, _timeProvider.GetUtcNow());
            await _db.Orders.AddAsync(order, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            var httpClient = _httpClientFactory.CreateClient("ReportAPI");
            _logger.LogInformation("order placed {Order}", order);
            var response = await httpClient.GetAsync($"/api/order/{order.Id}",cancellationToken);
            response.EnsureSuccessStatusCode();
            return order.Id;
        }

        private async Task<ErrorOr<IReadOnlyList<OrderItem>>> CreateOrderItems(
            IReadOnlyList<CreateOrderItemRequest> items, CancellationToken cancellationToken)
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

        private static Func<CreateOrderItemRequest, ErrorOr<OrderItem>> CreateOrderItem(
            Dictionary<Guid, Product> products)
        {
            return i =>
            {
                var quantity = OrderItemQuantity.Create(i.Quantity);

                if (quantity.IsError)
                    return quantity.Errors;

                var product = products[i.ProductId];
                var productQuantity = ProductQuantity.Create(i.Quantity);
                if (productQuantity.IsError) return productQuantity.Errors;
                product.UpdateQuantity(product.ProductQuantity - productQuantity.Value);
                return new OrderItem(product.Id, quantity.Value, product.Price);
            };
        }
    }
}