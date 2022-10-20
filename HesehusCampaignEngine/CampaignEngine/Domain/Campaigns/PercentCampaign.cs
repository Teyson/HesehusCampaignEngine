using System.Collections.Generic;

namespace CampaignEngine.Domain.Campaigns
{
    public class PercentCampaign : Campaign
    {
        public PercentCampaign(string id, HashSet<Product> affectedProducts, int productsToActivate, decimal discountPercentage)
            : base(id, affectedProducts, productsToActivate)
        {
            DiscountPercentage = discountPercentage;
        }
        
        public decimal DiscountPercentage { get; set; }
    }
}