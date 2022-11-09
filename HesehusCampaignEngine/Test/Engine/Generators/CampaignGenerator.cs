using System.Collections.Generic;
using System.Linq;
using CampaignEngine.Domain;
using CampaignEngine.Domain.Campaigns;
using FsCheck;

namespace Test.Engine.Generators
{
    public static class CampaignGenerator
    {
        public static Gen<Campaign> CampaignGen(List<Product> allProducts)
        {
            Gen<Campaign>[] gens = {DiscountCampaignGen(allProducts), PercentCampaignGen(allProducts), SetPriceCampaignGen(allProducts)};
            return Gen.OneOf(gens);
        }
        
        private static Gen<Campaign> DiscountCampaignGen(List<Product> allProducts)
        {
            Gen<IList<Product>> subListGen = Gen.SubListOf(allProducts.ToArray());
            var idGen = Arb.Default.String().Generator;
            var minActivation = 1;

            return from products in subListGen
                from id in idGen
                from activation in Gen.Choose(minActivation, products.Count + 2)
                from discount in Arb.Default.Decimal().Generator.Where(x => x < products.Sum(product => product.Price))
                select (Campaign)new DiscountCampaign(id, products.ToHashSet(), activation, discount);
        }

        private static Gen<Campaign> PercentCampaignGen(List<Product> allProducts)
        {
            Gen<IList<Product>> subListGen = Gen.SubListOf(allProducts.ToArray());
            var idGen = Arb.Default.String().Generator;
            var minActivation = 1;
            
            return from products in subListGen
                from id in idGen
                from activation in Gen.Choose(minActivation, products.Count + 2)
                from discount in Arb.Default.Decimal().Generator.Where(x => x < 100)
                select (Campaign)new PercentCampaign(id, products.ToHashSet(), activation, discount);
        }

        private static Gen<Campaign> SetPriceCampaignGen(List<Product> allProducts)
        {
            Gen<IList<Product>> subListGen = Gen.SubListOf(allProducts.ToArray());
            var idGen = Arb.Default.String().Generator;
            var minActivation = 1;
            
            return from products in subListGen
                from id in idGen
                from activation in Gen.Choose(minActivation, products.Count + 2)
                from newTotalPrice in Arb.Default.Decimal().Generator.Where(x => x < products.Sum(product => product.Price))
                select (Campaign)new SetPriceCampaign(id, products.ToHashSet(), activation, newTotalPrice);
        }
    }
}