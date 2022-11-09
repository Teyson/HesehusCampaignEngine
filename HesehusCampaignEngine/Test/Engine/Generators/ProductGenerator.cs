using System;
using System.Collections.Generic;
using System.Linq;
using CampaignEngine.Domain;
using FsCheck;

namespace Test.Engine.Generators
{
    public class ProductGenerator
    {
        public static Arbitrary<IList<Product>> Generate() {
            var minPrice = 1;
            var maxPrice = 1000;
            var priceGen = Gen.Elements(Enumerable.Range(minPrice, maxPrice).Select(i => (decimal)(i/10D)));
            Func<int, int> fun = i => i + 10;
            var idGen = Arb.Default.String().Generator;
    
            var productGen = from price in priceGen
                from id in idGen.Where(x => x == null || x.Length > 3)
                select new Product(id, price);

            var productListGen = productGen.ListOf();
            
            var arb = Arb.From(productListGen);

            return arb;
        }
    }
}