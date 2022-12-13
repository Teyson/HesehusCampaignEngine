using System.Collections.Generic;
using CampaignEngine.Domain.Products;

namespace CampaignEngine.Domain.Campaigns
{
    public class PercentCampaign : Campaign
    {
        public PercentCampaign(string id, HashSet<Product> affectedProducts, int productsToActivate, decimal discountPercentage)
            : base(id, affectedProducts, productsToActivate)
        {
            DiscountPercentage = discountPercentage;
        }
        
        public override decimal CalculatePrice(HashSet<Product> activatedProducts)
        {
            var sum = activatedProducts.Sum(x => x.Price);
            return sum - (sum * DiscountPercentage / 100);
        }
        
        public decimal DiscountPercentage { get; set; }
    }
}