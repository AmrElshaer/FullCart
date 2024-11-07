using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Data;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Orders.Commands.CreateOrder.CreateOrderV2;

public class CreateOrderCommandHandlerV2 : IAlternativeHandler<CreateOrderCommand, ErrorOr<CreateOrderResponse>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICartDbContext _db;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CreateOrderCommandHandlerV2> _logger;

    public CreateOrderCommandHandlerV2(ICartDbContext db, ICurrentUserProvider currentUserProvider,
        TimeProvider timeProvider,ILogger<CreateOrderCommandHandlerV2> logger)
    {
        _db = db;
        _currentUserProvider = currentUserProvider;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateOrderResponse>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create Order CreateOrderCommandHandlerV2");
        var orderItems = await CreateOrderItems(request.Items, cancellationToken);
        if (orderItems.IsError)
            return orderItems.Errors;

        var user = _currentUserProvider.GetCurrentUser();
        var userId = user.Id;
        var order = new Order(userId, orderItems.Value, _timeProvider.GetUtcNow());
        await _db.Orders.AddAsync(order, cancellationToken);
        return new CreateOrderResponse { OrderId = order.Id.Value };
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