namespace VendingMachine;

public class Inventory
{
    private readonly List<ProductLine> _productLines;

    public Inventory(List<ProductLine> productLines)
    {
        _productLines = productLines;
    }
    
    // public List<ProductLine> Products { get; } = new()
    // {
    //     new( new ProductItem("Cola", 1m), 2),
    //     new(new ProductItem("Crisps",0.5m), 1),
    //     new(new ProductItem("Chocolate", 0.65m), 3)
    // };

    public bool IsProductAvailable(string productName) =>
        _productLines.Any(p => p.Product.Name == productName && p.Quantity > 0);

    public decimal GetProductPrice(string productName) => _productLines.First(p => p.Product.Name == productName).Product.Price;

    public ProductItem DispenseProduct(string productName)
    {
        var product = _productLines.First(p => p.Product.Name == productName);
        product.Quantity--;
        return product.Product;
    }
}