using Domain.Products;

namespace Domain.Orders;

public class OrderItem
{
    public OrderItemQuantity Quantity { get; private set; }= default!;

    public Guid ProductId { get; private set; }

    public ProductPrice ProductPrice { get; set; }= default!;

    public Product Product { get; private set; } = default!;

    private OrderItem()
    {
        
    }

    public OrderItem(Guid productId,OrderItemQuantity quantity,ProductPrice productPrice)
    {
        ProductId = productId;
        ProductPrice = productPrice;
        Quantity = quantity;

    }
}
