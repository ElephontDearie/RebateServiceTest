using System;
using System.Collections.Generic;
using System.Net.Cache;
using CommandLine;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options => 
            {
                var request = new CalculateRebateRequest
                {
                    RebateIdentifier = options.RebateId,
                    ProductIdentifier = options.ProductId
                    
                };

                if (options.Volume.HasValue)
                {
                    request.Volume = options.Volume.Value;
                }
                RunProgramAndReturn(request);
            })
            .WithNotParsed<Options>(errs => HandleParseErrors(errs));
    }

    static void RunProgramAndReturn(CalculateRebateRequest request)
    {   
        var service = new RebateService(new RebateDataStore(), new ProductDataStore());
        var result = service.Calculate(request);
        
        Console.WriteLine(result);
        if (result.Success) 
        {
            Console.WriteLine($"Success! Rebate calculated as {result.RebateAmount} is stored.");
        } 
        else {
            Console.WriteLine("Failure! Could not calculate rebate!");
        }
    }

    static void HandleParseErrors(IEnumerable<Error> errs)
    {
        // Add any logic for handling errors
        foreach (var err in errs)
        {
            Console.WriteLine($"Error parsing argument: {err.Tag}");

        }
    }
}
