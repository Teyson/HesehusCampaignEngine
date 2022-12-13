using System.Diagnostics;
using System.Globalization;
using CampaignEngine.Domain;
using CampaignEngine.Domain.Campaigns;
using CampaignEngine.Domain.Products;
using CsvHelper;
using CsvHelper.Configuration;
using Test.Helpers;

namespace Test.Engine;

public class CampaignEngineTest
{
    private const int AmountOfProductsLowerBound = 2;
    private const int AmountOfProductsUpperBound = 7;
    private const int AmountOfCampaignsLowerBound = 2;
    private const int AmountOfCampaignsUpperBound = 10;

    public static void GenerateTestCase()
    {
        var stopwatch = Stopwatch.StartNew();

        for (var k = 0; k < 1000; k++)
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
            var result = engine.CalculatePrice(orderLines, campaigns);
            var timeElapsed = stopwatch.ElapsedMilliseconds;

            if (engine.CampaignActivations.Count < 8) continue;
            
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false,
            };

            using (var stream = File.Open(
                       "C:\\Users\\teis\\OneDrive\\Dokumenter\\Skole\\SDU\\Software\\7. semester\\4. Development\\HesehusCampaignEngine\\HesehusCampaignEngine\\Test\\TestCases_v2.csv",
                       FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(new List<TestResult>
                {
                    new(
                        engine.CampaignActivations.Count,
                        timeElapsed,
                        engine.BasketActivations.Count,
                        AmountOfProductsLowerBound,
                        AmountOfProductsUpperBound,
                        AmountOfCampaignsLowerBound,
                        AmountOfCampaignsUpperBound,
                        seed)
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
            var result = engine.CalculatePrice(orderLines, campaigns);
            var timeElapsed = stopwatch.ElapsedMilliseconds;
            
            testResults.Add(new TestResult(engine.CampaignActivations.Count,
                timeElapsed,
                engine.BasketActivations.Count,
                AmountOfProductsLowerBound,
                AmountOfProductsUpperBound,
                AmountOfCampaignsLowerBound,
                AmountOfCampaignsUpperBound,
                testCase.Seed));
        }
        
        using (var writer = new StreamWriter($"C:\\Users\\teis\\OneDrive\\Dokumenter\\Skole\\SDU\\Software\\7. semester\\4. Development\\HesehusCampaignEngine\\HesehusCampaignEngine\\Test\\TestResults_v2_{Guid.NewGuid()}.csv"))
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
        };

        using (var reader =
               new StreamReader(
                   "C:\\Users\\teis\\OneDrive\\Dokumenter\\Skole\\SDU\\Software\\7. semester\\4. Development\\HesehusCampaignEngine\\HesehusCampaignEngine\\Test\\TestCases.csv"))
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
            var keepRngSeed = rng.Next(1, products.Count);
            var numberOfAffectedProducts = products.Count / 2;
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