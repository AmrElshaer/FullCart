using Domain.Products;

namespace Domain.Orders;

public class OrderItem
{
    public OrderItemQuantity Quantity { get; private set; }

    public Guid ProductId { get; private set; }

    public ProductPrice ProductPrice { get; set; }

    public Product Product { get; private set; } = default!;

    public OrderItem(Guid productId,OrderItemQuantity quantity,ProductPrice productPrice)
    {
        ProductId = productId;
        ProductPrice = productPrice;
        Quantity = quantity;

    }
}
