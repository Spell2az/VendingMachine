namespace VendingMachine;

public class ProductItem
{
    public ProductItem(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public string Name { get; }
    public decimal Price { get; }
}