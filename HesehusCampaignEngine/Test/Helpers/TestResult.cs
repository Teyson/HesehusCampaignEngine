namespace Test.Helpers;

public class TestResult
{
    public int N { get; set; }
    public long RunTime { get; set; }
    public int AmountOfBaskets { get; set; }
    public int AmountOfProductsLowerBound {get; set;}
    public int AmountOfProductsUpperBound {get; set;}
    public int AmountOfCampaignsLowerBound {get; set;}
    public int AmountOfCampaignsUpperBound {get; set;}
    public int Seed { get; set; }

    public TestResult(
        int n,
        long runTime,
        int amountOfBaskets,
        int amountOfProductsLowerBound,
        int amountOfProductsUpperBound,
        int amountOfCampaignsLowerBound,
        int amountOfCampaignsUpperBound,
        int seed)
    {
        N = n;
        AmountOfBaskets = amountOfBaskets;
        RunTime = runTime;
        Seed = seed;
        AmountOfProductsLowerBound = amountOfProductsLowerBound;
        AmountOfProductsUpperBound = amountOfProductsUpperBound;
        AmountOfCampaignsLowerBound = amountOfCampaignsLowerBound;
        AmountOfCampaignsUpperBound = amountOfCampaignsUpperBound;
    }
}