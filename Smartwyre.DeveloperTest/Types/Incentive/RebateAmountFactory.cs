using System;

namespace Smartwyre.DeveloperTest.Types.Incentive;

public class RebateAmountFactory
{
    public CalculateRebateResult GetRebateResultType(IncentiveType incentiveType)
    {
        return incentiveType switch
        {
            IncentiveType.FixedCashAmount => new FixedCashAmount(),
            IncentiveType.FixedRateRebate => new FixedRateRebate(),
            IncentiveType.AmountPerUom => new AmountPerUom(),
            _ => throw new ArgumentOutOfRangeException(nameof(incentiveType), "Invalid incentive Type provided.")
        };
    }
}