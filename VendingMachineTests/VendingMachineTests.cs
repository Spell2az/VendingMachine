namespace VendingMachineTests;

public class VendingMachineTests
{
    [Fact]
    public void WhenValidCoinsAreInserted_CoinsAreAcceptedAndDisplayUpdated()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine = new VendingMachine.VendingMachine(new CoinHolder(), new Inventory(new List<ProductLine>()),
            displayProvider);

        vendingMachine.InsertCoin(0.1m);
        vendingMachine.InsertCoin(0.01m);

        displayProvider.ClearReceivedCalls();
        vendingMachine.InsertCoin(1m);

        displayProvider.Received(1).DisplayStatus(VendingStatus.COIN_INSERTED);
        displayProvider.Received(1).DisplayCurrentAmount(1.11m);
        vendingMachine.CurrentAmount.Should().Be(1.11m);
    }

    [Fact]
    public void WhenInValidCoinsAreInserted_InvalidCoinsAreReturned()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine = new VendingMachine.VendingMachine(new CoinHolder(), new Inventory(new List<ProductLine>()),
            displayProvider);
        displayProvider.ClearReceivedCalls();
        vendingMachine.InsertCoin(0.3m);

        displayProvider.Received(1).DisplayStatus(VendingStatus.INSERT_COIN);
        displayProvider.Received(1).DisplayCurrentAmount(0m);

        vendingMachine.CurrentAmount.Should().Be(0);
        vendingMachine.CoinReturn.Should().BeEquivalentTo(new List<Coin> { new(0.3m) });
    }

    [Fact]
    public void WhenValidCoinsAreInserted_AndReturnCoinsIsCalled_InsertedCoinsAreReturned()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine = new VendingMachine.VendingMachine(new CoinHolder(), new Inventory(new List<ProductLine>()),
            displayProvider);

        vendingMachine.InsertCoin(0.1m);
        vendingMachine.InsertCoin(0.01m);
        vendingMachine.InsertCoin(1m);

        displayProvider.ClearReceivedCalls();

        vendingMachine.InsertCoin(1m);

        displayProvider.Received(1).DisplayStatus(VendingStatus.COIN_INSERTED);
        displayProvider.Received(1).DisplayCurrentAmount(2.11m);

        displayProvider.ClearReceivedCalls();
        vendingMachine.ReturnCoins();

        displayProvider.Received(1).DisplayStatus(VendingStatus.INSERT_COIN);
        displayProvider.Received(1).DisplayCurrentAmount(0m);

        vendingMachine.CoinReturn
            .Should()
            .BeEquivalentTo(new List<Coin> { new(0.1m), new(0.01m), new(1m), new(1m)});
    }

    [Fact]
    public void WhenSelectedProductIsUnavailable_SoldOutIsDisplayed()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine =
            new VendingMachine.VendingMachine(new CoinHolder(),
                new Inventory(new List<ProductLine> { new ProductLine(new ProductItem("Cola", 1m), 0) }),
                displayProvider); 
        vendingMachine.InsertCoin(2m);
        displayProvider.ClearReceivedCalls();
        vendingMachine.TryDispenseProduct("Cola");
        
        displayProvider.Received(1).DisplayStatus(VendingStatus.SOLD_OUT);
        displayProvider.Received(1).DisplayCurrentAmount(2m);
        
        displayProvider.ClearReceivedCalls();
        vendingMachine.CheckDisplay();
        
        displayProvider.Received(1).DisplayStatus(VendingStatus.COIN_INSERTED);
        displayProvider.Received(1).DisplayCurrentAmount(2m);
    }
    
    [Fact]
    public void WhenCorrectAmountIsInserted_ProductIsDispensedAndCorrectMessageDisplayed()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine =
            new VendingMachine.VendingMachine(new CoinHolder(),
                new Inventory(new List<ProductLine> { new ProductLine(new ProductItem("Cola", 1m), 1) }),
                displayProvider); 
        vendingMachine.InsertCoin(1m);
        displayProvider.ClearReceivedCalls();
        vendingMachine.TryDispenseProduct("Cola");
        
        displayProvider.Received(1).DisplayStatus(VendingStatus.THANK_YOU);
        displayProvider.Received(1).DisplayCurrentAmount(0);
        vendingMachine.ProductChute.Should().BeEquivalentTo(new ProductItem("Cola", 1m));
        
        displayProvider.ClearReceivedCalls();
        vendingMachine.CheckDisplay();
        
        displayProvider.Received(1).DisplayStatus(VendingStatus.INSERT_COIN);
        displayProvider.Received(1).DisplayCurrentAmount(0m);
    }

    [Fact]
    public void WhenNoCoinsAreInsertedAndProductIsSelected_ProductPriceIsDisplayed()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine =
            new VendingMachine.VendingMachine(new CoinHolder(),
                new Inventory(new List<ProductLine> { new ProductLine(new ProductItem("Cola", 1m), 1) }),
                displayProvider); 
        displayProvider.ClearReceivedCalls();
        
        vendingMachine.TryDispenseProduct("Cola");
        displayProvider.Received(1).DisplayStatus(VendingStatus.PRICE);
        displayProvider.Received(1).DisplayCurrentAmount(1m);
        
        displayProvider.ClearReceivedCalls();
        
        vendingMachine.CheckDisplay();
        displayProvider.Received(1).DisplayStatus(VendingStatus.INSERT_COIN);
        displayProvider.Received(1).DisplayCurrentAmount(0m);
    }
    
    [Fact]
    public void WhenInsufficientCoinsAreInsertedAndProductIsSelected_ProductPriceIsDisplayed()
    {
        var displayProvider = Substitute.For<IDisplayProvider>();
        var vendingMachine =
            new VendingMachine.VendingMachine(new CoinHolder(),
                new Inventory(new List<ProductLine> { new ProductLine(new ProductItem("Cola", 1m), 1) }),
                displayProvider); 
        displayProvider.ClearReceivedCalls();
        
        vendingMachine.InsertCoin(0.2m);
        vendingMachine.TryDispenseProduct("Cola");
        displayProvider.Received(1).DisplayStatus(VendingStatus.PRICE);
        displayProvider.Received(1).DisplayCurrentAmount(1m);
        
        displayProvider.ClearReceivedCalls();
        
        vendingMachine.CheckDisplay();
        displayProvider.Received(1).DisplayStatus(VendingStatus.COIN_INSERTED);
        displayProvider.Received(1).DisplayCurrentAmount(0.2m);
    }
}