using System.Diagnostics;
using System.Globalization;
using CampaignEngine.Domain.Campaigns;
using CampaignEngine.Domain.Products;
using CsvHelper;
using CsvHelper.Configuration;
using Test.Helpers;

namespace Test.Engine;

public class CampaignEngineTest
{
    private const int AmountOfProductsLowerBound = 2;
    private const int AmountOfProductsUpperBound = 10;
    private const int AmountOfCampaignsLowerBound = 2;
    private const int AmountOfCampaignsUpperBound = 10;

    public static void GenerateTestCase()
    {
        
        var stopwatch = Stopwatch.StartNew();

        for (var k = 0; k < 10000; k++)
        {
            var seed = Guid.NewGuid().GetHashCode();
            var rng = new Random(seed);

            var amountOfProducts = rng.Next(AmountOfProductsLowerBound, AmountOfProductsUpperBound);
            var amountOfCampaigns = rng.Next(AmountOfCampaignsLowerBound, AmountOfCampaignsUpperBound);

            var products = GenerateProducts(amountOfProducts, rng);

            var campaigns = GenerateCampaigns(amountOfCampaigns, rng, products);

            var orderLines = OrderLines(products, rng);

            var engine = new CampaignEngine.Engine.Engine();
            stopwatch.Restart();
            var result = engine.CalculatePrice(orderLines, campaigns, -1);
            var timeElapsed = stopwatch.ElapsedMilliseconds;
            var currentProcess = Process.GetCurrentProcess();
            var totalBytesOfMemoryUsed = currentProcess.WorkingSet64;

            var n = engine.CampaignActivations.Count;
            if (timeElapsed <= 0 || n < 8 || n > 100) continue;
            
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false,
            };

            using (var stream = File.Open(
                       "C:\\Users\\teis\\OneDrive\\Dokumenter\\Skole\\SDU\\Software\\7. semester\\4. Development\\HesehusCampaignEngine\\HesehusCampaignEngine\\Test\\Mass_test_v2.csv",
                       FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(new List<TestResult>
                {
                    new(
                        n,
                        timeElapsed,
                        engine.BasketActivations.Count,
                        AmountOfProductsLowerBound,
                        AmountOfProductsUpperBound,
                        AmountOfCampaignsLowerBound,
                        AmountOfCampaignsUpperBound,
                        seed){TotalBytesOfMemoryUsed = totalBytesOfMemoryUsed}
                });
            }
        }
    }

    public static void RunTestCase()
    {
        var stopwatch = new Stopwatch();
        List<TestResult> testResults = new List<TestResult>();
        
        var testCases = ReadFile();

        foreach (var testCase in testCases)
        {
            var rng = new Random(testCase.Seed);

            var amountOfProducts = rng.Next(testCase.AmountOfProductsLowerBound, testCase.AmountOfProductsUpperBound);
            var amountOfCampaigns = rng.Next(testCase.AmountOfCampaignsLowerBound, testCase.AmountOfCampaignsUpperBound);

            var products = GenerateProducts(amountOfProducts, rng);

            var campaigns = GenerateCampaigns(amountOfCampaigns, rng, products);

            var orderLines = OrderLines(products, rng);

            var engine = new CampaignEngine.Engine.Engine();
            stopwatch.Restart();
            var result = engine.CalculatePrice(orderLines, campaigns, -1);
            var timeElapsed = stopwatch.ElapsedMilliseconds;
            var currentProcess = Process.GetCurrentProcess();
            var totalBytesOfMemoryUsed = currentProcess.WorkingSet64;

            
            testResults.Add(new TestResult(engine.CampaignActivations.Count,
                timeElapsed,
                engine.BasketActivations.Count,
                testCase.AmountOfProductsLowerBound,
                testCase.AmountOfProductsUpperBound,
                testCase.AmountOfCampaignsLowerBound,
                testCase.AmountOfCampaignsUpperBound,
                testCase.Seed) {TotalBytesOfMemoryUsed = totalBytesOfMemoryUsed});
        }
        
        using (var writer = new StreamWriter($"C:\\Users\\teis\\OneDrive\\Dokumenter\\Skole\\SDU\\Software\\7. semester\\4. Development\\HesehusCampaignEngine\\HesehusCampaignEngine\\Test\\Mass_test_v2_with_memory.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
        {
            csv.WriteRecords(testResults);
        }
    }

    private static List<TestResult> ReadFile()
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        using (var reader =
               new StreamReader(
                   "C:\\Users\\teis\\OneDrive\\Dokumenter\\Skole\\SDU\\Software\\7. semester\\4. Development\\HesehusCampaignEngine\\HesehusCampaignEngine\\Test\\TestCases_After_Bugfix.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            return csv.GetRecords<TestResult>().ToList();
        }
    }

    private static List<OrderLine> OrderLines(List<Product> products, Random rng)
    {
        var orderLines = new List<OrderLine>();
        foreach (var product in products)
        {
            orderLines.Add(new OrderLine(product, rng.Next(1, 3)));
        }

        return orderLines;
    }

    private static List<Product> GenerateProducts(int amountOfProducts, Random rng)
    {
        var products = new List<Product>();
        var priceScalar = 100;

        for (var i = 0; i < amountOfProducts; i++)
        {
            products.Add(
                new Product(RandomHelpers.RandomString(5, rng), (decimal) (rng.NextDouble() * priceScalar)));
        }

        return products;
    }

    private static HashSet<Campaign> GenerateCampaigns(int amountOfCampaigns, Random rng, List<Product> products)
    {
        var campaigns = new HashSet<Campaign>();

        for (var i = 0; i < amountOfCampaigns; i++)
        {
            var numberOfAffectedProducts = rng.Next(1, products.Count);
            var affectedProducts = products.OrderBy(x => rng.Next()).Take(numberOfAffectedProducts).ToHashSet();
            var productsToActivate = rng.Next(1, affectedProducts.Count);

            var campaign = GenerateCampaign(rng, affectedProducts, productsToActivate);

            campaigns.Add(campaign);
        }

        return campaigns;
    }

    private static Campaign GenerateCampaign(Random rng, HashSet<Product> affectedProducts, int productsToActivate)
    {
        Campaign campaign = null!;
        var campaignToGenerate = rng.Next(0, 2);
        switch (campaignToGenerate)
        {
            case 0:
                campaign = new DiscountCampaign(
                    RandomHelpers.RandomString(5, rng),
                    affectedProducts,
                    productsToActivate,
                    (decimal) rng.NextDouble() * affectedProducts.Sum(x => x.Price));
                break;
            case 1:
                campaign = new PercentCampaign(
                    RandomHelpers.RandomString(5, rng),
                    affectedProducts,
                    productsToActivate,
                    (decimal) (rng.NextDouble() * 100));
                break;
            case 2:
                campaign = new SetPriceCampaign(
                    RandomHelpers.RandomString(5, rng),
                    affectedProducts,
                    productsToActivate,
                    (decimal) rng.NextDouble() * affectedProducts.Sum(x => x.Price));
                break;
        }

        return campaign;
    }
}