namespace VendingMachine;

public class ProductLine
{
    public ProductLine(ProductItem productItem, int quantity)
    {
        Product = productItem;
        Quantity = quantity;
    }

    public ProductItem Product{ get; set; }

    public int Quantity { get; set; }
    
}