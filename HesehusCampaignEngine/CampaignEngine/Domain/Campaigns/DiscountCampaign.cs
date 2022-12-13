using System.Collections.Generic;
using CampaignEngine.Domain.Products;

namespace CampaignEngine.Domain.Campaigns
{
    public class DiscountCampaign : Campaign
    {
        public DiscountCampaign(string id, HashSet<Product> affectedProducts, int productsToActivate, decimal discount)
            : base(id, affectedProducts, productsToActivate)
        {
            Discount = discount;
        }
        
        public override decimal CalculatePrice(HashSet<Product> activatedProducts)
        {
            var sum1 = activatedProducts.Sum(x => x.Price);
            return sum1 - Discount;
        }

        public decimal Discount { get; set; }
    }
}