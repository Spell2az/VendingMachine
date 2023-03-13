namespace VendingMachine;

public class CoinHolder
{
    private static readonly List<decimal> CoinValues = new()
        { 0.01m, 0.02m, 0.05m, 0.1m, 0.2m, 0.5m, 1m, 2m };

    public static bool IsCoinValid(decimal coinValue) => CoinValues.Contains(coinValue);
    
    public readonly Dictionary<decimal, int> Coins = new()
    {
        {0.01m, 0},
        {0.02m, 0},
        {0.05m, 0},
        {0.10m, 0},
        {0.20m, 0},
        {0.50m, 0},
        {1.00m, 0},
        {2.00m, 0}
    };
    
    public void AddCoin(decimal value) => Coins[value] += 1;

    public void DepositCoins(CoinHolder coinsToDeposit) =>
        coinsToDeposit.Coins.Keys.ToList().ForEach(c => Coins[c] += coinsToDeposit.Coins[c]);

}
