using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Types.Incentive;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly RebateAmountFactory _factory = new();
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;

    public RebateService(IRebateDataStore rebateStore, IProductDataStore productStore)
    {
        _rebateDataStore = rebateStore;
        _productDataStore = productStore;
    }
    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate == null || product == null) {
            return new CalculateRebateResult
            {
                Success = false
            };
        }

        var rebateAmountCalculator = _factory.GetRebateResultType(rebate.Incentive);
        var rebateAmount = rebateAmountCalculator.GetRebateAmount(rebate, product, request.Volume);

        if (rebateAmountCalculator.Success)
        {
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return rebateAmountCalculator;
    }
}
