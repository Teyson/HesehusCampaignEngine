using System.Collections.Generic;
using CampaignEngine.Domain;
using FsCheck;

namespace Test.Engine.Generators
{
    public static class OrderLineGenerator
    {
        public static List<OrderLine> Generate(List<Product> allProducts)
        {
            var minAmount = 0;
            var maxAmount = 10;
            var amountGen = Gen.Choose(minAmount, maxAmount);

            List<OrderLine> orderLines = new();
            foreach (var product in allProducts)
            {
                var orderLine = (from amount in amountGen
                    select new OrderLine(product, amount)).Sample(5,1)[0];
                
                orderLines.Add(orderLine);
            }

            return orderLines;
        }
    }
}