namespace VendingMachine;

public class VendingMachine
{
    private readonly IDisplayProvider _displayProvider;

    private CoinHolder _coinsInserted = new CoinHolder();

    public ProductItem? ProductChute;
    private CoinHolder CashBox { get; }
    private Inventory Inventory { get; }
    private string Status { get; set; } = VendingStatus.INSERT_COIN;
    public List<Coin> CoinReturn { get; private set; } = new();
    public decimal CurrentAmount { get; private set; }

    public VendingMachine(CoinHolder cashBox, Inventory inventory, IDisplayProvider displayProvider)
    {
        _displayProvider = displayProvider;
        CashBox = cashBox;
        Inventory = inventory;
        UpdateDisplay(Status, CurrentAmount);
    }
    public void InsertCoin(decimal coinValue)
    {
        if (CoinHolder.IsCoinValid(coinValue))
        {
            _coinsInserted.AddCoin(coinValue);
            CurrentAmount += coinValue;
            Status = VendingStatus.COIN_INSERTED;
            UpdateDisplay(Status, CurrentAmount);
            return;
        }

        UpdateDisplay(Status, CurrentAmount);
        CoinReturn.Add(new Coin(coinValue));
    }

    private void UpdateDisplay(string status, decimal amount)
    {
        _displayProvider.DisplayStatus(status);
        _displayProvider.DisplayCurrentAmount(amount);
    }

    public void ReturnCoins()
    {
        CoinReturn = _coinsInserted.Coins.Keys
            .Where(c => _coinsInserted.Coins[c] > 0).Select(c =>
                Enumerable.Range(0, _coinsInserted.Coins[c]).Select(_ => new Coin(c)))
            .SelectMany(cs => cs)
            .ToList();
        _coinsInserted = new CoinHolder();
        CurrentAmount = 0;
        Status = VendingStatus.INSERT_COIN;
        UpdateDisplay(Status, CurrentAmount);
    }

    public void TryDispenseProduct(string productName)
    {
        var isProductAvailable = Inventory.IsProductAvailable(productName);
        if (!isProductAvailable)
        {
            UpdateDisplay(VendingStatus.SOLD_OUT, CurrentAmount);
            return;
        }

        var productPrice = Inventory.GetProductPrice(productName);

        if (productPrice > CurrentAmount)
        {
            UpdateDisplay(VendingStatus.PRICE, productPrice);
            return;
        }

        if (productPrice == CurrentAmount)
        {
            ProductChute = Inventory.DispenseProduct(productName);
            CashBox.DepositCoins(_coinsInserted);
            UpdateDisplay(VendingStatus.THANK_YOU, 0m);

            Status = VendingStatus.INSERT_COIN;
            CurrentAmount = 0m;
            _coinsInserted = new CoinHolder();
        }

        if (CurrentAmount > productPrice)
        {
            //Check if change is available
        }
    }

    public void CheckDisplay() => UpdateDisplay(Status, CurrentAmount);
}

public interface IDisplayProvider
{
    public void DisplayCurrentAmount(decimal amount);
    public void DisplayStatus(string status);
}

public static class VendingStatus
{
    public const string INSERT_COIN = "INSERT COIN";
    public const string SOLD_OUT = "SOLD_OUT";
    public const string PRICE = "PRICE";
    public const string THANK_YOU = "THANK YOU";
    public const string COIN_INSERTED = "";
}