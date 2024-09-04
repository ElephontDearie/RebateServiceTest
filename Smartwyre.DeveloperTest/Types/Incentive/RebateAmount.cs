namespace Smartwyre.DeveloperTest.Types.Incentive;


public class FixedCashAmount : CalculateRebateResult
{
    public override decimal Calculate(Rebate rebate, Product product, decimal volume = 0m)
    {
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount)) {
            return Failure();
        }
        this.Success = true;
        return rebate.Amount;
    }
}

public class FixedRateRebate : CalculateRebateResult
{
    public override decimal Calculate(Rebate rebate, Product product, decimal volume = 0m)
    {
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) || rebate.Percentage == 0m || product.Price == 0m || volume == 0m)
        {
            return Failure();
        }
        return MultiplyWithErrorHandling(product.Price, rebate.Percentage, volume);
    }
}

public class AmountPerUom : CalculateRebateResult
{
    public override decimal Calculate(Rebate rebate, Product product, decimal volume = 0m)
    {
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) || rebate.Amount == 0m || volume == 0m)
        {
            return Failure();
        }
        return MultiplyWithErrorHandling(rebate.Amount, volume);
    }
}