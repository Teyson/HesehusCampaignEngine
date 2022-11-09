using System.Linq;
using FsCheck;

namespace Test.Engine.Generators
{
    public static class TestInstanceGenerator
    {
        public static TestInstance Generate()
        {
            var seed = FsCheck.Random.newSeed();
            
            var productGen = ProductGenerator.Generate().Generator;

            var products = productGen.Sample(5, 1).Head.ToList();

            var orderLines = OrderLineGenerator.Generate(products);
            
            var campaignGen =  CampaignGenerator.CampaignGen(products);

            var campaigns = campaignGen.Sample(5, 5).ToList();

            return new TestInstance(orderLines, campaigns);
        }
    }
}