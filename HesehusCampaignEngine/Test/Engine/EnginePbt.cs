using System.Collections.Generic;
using System.Linq;
using CampaignEngine.Domain;
using FsCheck;
using FsCheck.NUnit;
using Test.Engine.Generators;

namespace Test.Engine
{
    public class EnginePbt
    {
        [Property]
        public Property Always_Better_Than_Or_Equal_Product_Price_Sum()
        {
            var testInstance = TestInstanceGenerator.Generate();
            
            var engine = new CampaignEngine.Engine.Engine();

            var output = engine.CalculatePrice(testInstance.OrderLines.ToList(), testInstance.Campaigns.ToHashSet());

            var productSum = testInstance.OrderLines.Sum(x => x.Amount * x.Product.Price);
            
            return (productSum >= output.Total).ToProperty();
        }
    }
}