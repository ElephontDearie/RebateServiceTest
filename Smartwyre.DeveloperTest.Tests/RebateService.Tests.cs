using Xunit;
using Moq;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types.Incentive;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private readonly RebateAmountFactory _factory = new();
    private FixedCashAmount _cashCalculator;
    private FixedRateRebate _rateCalculator;
    private AmountPerUom _uomCalculator;

    public RebateServiceTests()
    {
        ResetInstances();
    }

    private void ResetInstances()
    {
        _cashCalculator = (FixedCashAmount)_factory.GetRebateResultType(IncentiveType.FixedCashAmount);
        _rateCalculator = (FixedRateRebate)_factory.GetRebateResultType(IncentiveType.FixedRateRebate);
        _uomCalculator = (AmountPerUom)_factory.GetRebateResultType(IncentiveType.AmountPerUom);
    }

    [Fact]
    public void Test_RebateService_Stores_Result_When_Happy_Path()
    {
        var mockRebateStore = new Mock<IRebateDataStore>();
        var mockProductStore = new Mock<IProductDataStore>();
        var rebateId = "reb1234";
        var productId = "pr1234";
        var rebate = new Rebate { Percentage = 0.10m, Incentive = IncentiveType.FixedCashAmount };
        mockRebateStore.Setup(ds => ds.GetRebate(rebateId)).Returns(rebate);
        mockProductStore.Setup(ds => ds.GetProduct(productId)).Returns(
            new Product { Price = 100.00m, SupportedIncentives = SupportedIncentiveType.FixedCashAmount }
        );

        var rebateService = new RebateService(mockRebateStore.Object, mockProductStore.Object);
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateId,
            ProductIdentifier = productId,
            Volume = 100.00m
        };
        var result = rebateService.Calculate(request);
        Assert.True(result.Success);
        mockRebateStore.Verify(store => store.GetRebate(rebateId), Times.Once);
        mockRebateStore.Verify(store => store.StoreCalculationResult(rebate, 0), Times.Once);
    }

    [Fact]
    public void Test_No_Stored_RebateService_Result_When_Unhappy_Path()
    {
        var mockRebateStore = new Mock<IRebateDataStore>();
        var mockProductStore = new Mock<IProductDataStore>();
        var rebateId = "reb-non-existent";
        var productId = "pr-non-existent";
        mockRebateStore.Setup(ds => ds.GetRebate(rebateId)).Returns((Rebate)null);
        mockProductStore.Setup(ds => ds.GetProduct(productId)).Returns((Product)null);
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateId,
            ProductIdentifier = productId,
            Volume = 100.00m
        };

        var rebateService = new RebateService(mockRebateStore.Object, mockProductStore.Object);
        var result = rebateService.Calculate(request); 
        Assert.False(result.Success);
        mockRebateStore.Verify(store => store.GetRebate(rebateId), Times.Once);
        mockRebateStore.Verify(store => store.StoreCalculationResult((Rebate)null, 0), Times.Never);
    }


    // TODO: Add tests to verify correct rebateAmount calculations for each incentive type are stored


    [Fact]
    public void Test_Zero_Calculation_For_RebateAmounts_When_Unsupported()
    {
        _cashCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.10m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            }, 
            100m
        );
        _rateCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.10m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            }, 
            100m
        );
        _uomCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.10m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            }, 
            100m
        );

        Assert.Equal(0m, _cashCalculator.RebateAmount);
        Assert.Equal(0m, _rateCalculator.RebateAmount);
        Assert.Equal(0m, _uomCalculator.RebateAmount);

    }

    [Fact]
    public void Test_Correct_Calculation_For_FixedCashAmount()
    {
        _cashCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.10m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            }, 
            100m
        );
        Assert.Equal(100m, _cashCalculator.RebateAmount);
        Assert.True(_cashCalculator.Success);
    }

    [Fact]
    public void Test_Correct_Calculation_For_FixedRateRebate()
    {
        _rateCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.50m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = 10m
            }, 
            100m
        );
        Assert.Equal(500m, _rateCalculator.RebateAmount);
        Assert.True(_rateCalculator.Success);

    }

    [Fact]
    public void Test_Correct_Calculation_For_AmountPerUom()
    {
        _uomCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.50m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom,
                Price = 10m
            }, 
            100m
        );
        Assert.Equal(10000m, _uomCalculator.RebateAmount);
        Assert.True(_uomCalculator.Success);
    }

    [Fact]
    public void Test_Zero_Calculation_When_No_Volume_Given_For_FixedRate_And_UOM()
    {
        _rateCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.50m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = 10m
            }
        );
        _uomCalculator.SetRebateAmount(
            new Rebate { 
                Amount = 100.00m, Percentage = 0.50m 
            }, 
            new Product {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom,
                Price = 10m
            }
        );

        Assert.Equal(0m, _rateCalculator.RebateAmount);
        Assert.Equal(0m, _uomCalculator.RebateAmount);
        Assert.False(_uomCalculator.Success);
        Assert.False(_rateCalculator.Success);

    }

}
