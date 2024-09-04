using CommandLine;

namespace Smartwyre.DeveloperTest.Runner;

public class Options
{
    [Option('r', "rebateId", Required = true, HelpText = "Specify the Rebate ID.")]
    public string RebateId { get; set; }

    [Option('p', "productId", Required = true, HelpText = "Specify the Product ID.")]
    public string ProductId { get; set; }

    [Option('v', "volume", Required = false, HelpText = "Specify the Volume.")]
    public decimal? Volume { get; set; }
}