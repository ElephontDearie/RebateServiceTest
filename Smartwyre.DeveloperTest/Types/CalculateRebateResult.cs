using System;
using System.Linq;

namespace Smartwyre.DeveloperTest.Types;

public class CalculateRebateResult
{
    public bool Success { get; set; }


    public decimal GetRebateAmount(Rebate rebate, Product product, decimal volume = 0m) 
    {
        // Add additional checks for general validity of rebate && product here before calculating for specific incentive types
        if (!Valid(volume)) {
            this.Success = false;
            return 0m;
        }

        var amount = Calculate(rebate, product, volume);
        return amount;

    }
    public virtual decimal Calculate(Rebate rebate, Product product, decimal volume = 0m) 
    {
        return rebate.Amount;
    }

    protected decimal Failure()
    {
        this.Success = false;
        return 0m;
    }

    protected decimal MultiplyWithErrorHandling(params decimal[] numbers)
    {
        this.Success = true;
        try
        {
            return checked(numbers.Aggregate(1m, (total, number) => total * number));
        }
        catch (OverflowException)
        {
           return Failure();
        }
    }

    private static bool Valid(decimal volume) 
    { 
        if (volume > 500m) // Enforce a maximum quantity cap for number of units that rebate can be claimed for.
        {
            return false;
        }
        return true;
    }



}
