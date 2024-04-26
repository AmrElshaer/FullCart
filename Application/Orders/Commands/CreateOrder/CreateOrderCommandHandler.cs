using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Orders;
using Domain.Products;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands.CreateOrder;

internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<Guid>>
{
    private readonly ICartDbContext _db;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateOrderCommandHandler(ICartDbContext db, ICurrentUserProvider currentUserProvider)
    {
        _db = db;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderItems = await CreateOrderItems(request.Items, cancellationToken);

        if (orderItems.IsError)
        {
            return orderItems.Errors;
        }

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
        {
            return ProductErrors.NotFoundProducts(notFoundProducts);
        }

        IReadOnlyList<ErrorOr<OrderItem>> orderItemsOrErrors = items
            .Select(CreateOrderItem(products)).ToList();

        var ordersItemsHaveErrors = orderItemsOrErrors.Where(i => i.IsError)
            .ToList();

        if (ordersItemsHaveErrors.Any())
        {
            return ordersItemsHaveErrors.SelectMany(i => i.Errors).ToList();
        }

        return orderItemsOrErrors.Select(oi => oi.Value)
            .ToList();
    }

    private static Func<CreateOrderItemRequest, ErrorOr<OrderItem>> CreateOrderItem(Dictionary<Guid, Product> products)
    {
        return i =>
        {
            var quantity = OrderItemQuantity.Create(i.Quantity);

            if (quantity.IsError)
            {
                return quantity.Errors;
            }

            var product = products[i.ProductId];

            return new OrderItem(product.Id, quantity.Value, product.Price);
        };
    }
}
