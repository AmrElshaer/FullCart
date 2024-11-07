﻿using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Data;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<CreateOrderResponse>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICartDbContext _db;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(ICartDbContext db, ICurrentUserProvider currentUserProvider,
        TimeProvider timeProvider,ILogger<CreateOrderCommandHandler> logger)
    {
        _db = db;
        _currentUserProvider = currentUserProvider;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateOrderResponse>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create Order CreateOrderCommandHandler");
        var orderItems = await CreateOrderItems(request.Items, cancellationToken);
        if (orderItems.IsError)
            return orderItems.Errors;

        var user = _currentUserProvider.GetCurrentUser();
        var userId = user.Id;
        var order = new Order(userId, orderItems.Value, _timeProvider.GetUtcNow());
        await _db.Orders.AddAsync(order, cancellationToken);
        var retryPolicy = Policy
            .Handle<DbUpdateConcurrencyException>()
            .RetryAsync(5, async (exception, retryCount) =>
            {
                foreach (var entry in ((DbUpdateConcurrencyException)exception).Entries)
                    if (entry.Entity is ProductQuantity)
                    {
                        var originValues = entry.OriginalValues;
                        var productId = (Guid)originValues["ProductId"];
                        var orderItem = orderItems.Value.FirstOrDefault(oi => oi.ProductId == productId);

                        var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                        if (databaseValues == null)
                            throw new Exception("The record you attempted to update was deleted by another user.");

                        var databaseQuantity = (int)databaseValues[nameof(ProductQuantity.Value)];
                        var newQuantity = databaseQuantity - orderItem!.Quantity.Quantity;

                        if (newQuantity < 0) throw new Exception("Insufficient product quantity.");

                        entry.CurrentValues[nameof(ProductQuantity.Value)] = newQuantity;
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                        entry.OriginalValues.SetValues(databaseValues);
                    }
            });

        // Execute the save operation with the retry policy
        await retryPolicy.ExecuteAsync(async () => { await _db.SaveChangesAsync(cancellationToken); });

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