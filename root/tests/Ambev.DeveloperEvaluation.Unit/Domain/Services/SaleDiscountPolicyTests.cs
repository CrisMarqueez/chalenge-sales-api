using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class SaleDiscountPolicyTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 0.10)]
    [InlineData(5, 0.10)]
    [InlineData(9, 0.10)]
    [InlineData(10, 0.20)]
    [InlineData(15, 0.20)]
    [InlineData(20, 0.20)]
    public void GetDiscountRate_ReturnsExpectedTier(int quantity, double expectedRate)
    {
        var rate = SaleDiscountPolicy.GetDiscountRate(quantity);
        Assert.Equal((decimal)expectedRate, rate);
    }

    [Fact]
    public void Apply_FourUnitsAtTen_ProducesTenPercentDiscount()
    {
        var line = new SaleLineItem
        {
            Quantity = 4,
            UnitPrice = 10m,
            IsCancelled = false
        };

        SaleDiscountPolicy.Apply(line);

        Assert.Equal(4m, line.DiscountAmount);
        Assert.Equal(36m, line.LineTotal);
    }

    [Fact]
    public void Apply_FiveUnitsAtTen_ProducesTenPercentDiscount()
    {
        var line = new SaleLineItem
        {
            Quantity = 5,
            UnitPrice = 10m,
            IsCancelled = false
        };

        SaleDiscountPolicy.Apply(line);

        Assert.Equal(5m, line.DiscountAmount);
        Assert.Equal(45m, line.LineTotal);
    }

    [Fact]
    public void Apply_TenUnitsAtHundred_ProducesTwentyPercentDiscount()
    {
        var line = new SaleLineItem
        {
            Quantity = 10,
            UnitPrice = 100m,
            IsCancelled = false
        };

        SaleDiscountPolicy.Apply(line);

        Assert.Equal(200m, line.DiscountAmount);
        Assert.Equal(800m, line.LineTotal);
    }

    [Fact]
    public void Apply_MoreThanMaxQuantity_Throws()
    {
        var line = new SaleLineItem
        {
            Quantity = 21,
            UnitPrice = 1m,
            IsCancelled = false
        };

        Assert.Throws<InvalidOperationException>(() => SaleDiscountPolicy.Apply(line));
    }

    [Fact]
    public void RecalculateLine_WhenCancelled_ZerosAmounts()
    {
        var line = new SaleLineItem
        {
            Quantity = 10,
            UnitPrice = 100m,
            IsCancelled = true,
            DiscountAmount = 999m,
            LineTotal = 999m
        };

        line.RecalculateLine();

        Assert.Equal(0m, line.DiscountAmount);
        Assert.Equal(0m, line.LineTotal);
    }
}
