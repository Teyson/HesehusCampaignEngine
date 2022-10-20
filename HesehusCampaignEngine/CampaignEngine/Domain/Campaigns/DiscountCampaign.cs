using System.Collections.Generic;

namespace HesehusCampaignEngine.Domain.Campaigns
{
    public class DiscountCampaign : Campaign
    {
        public DiscountCampaign(string id, HashSet<Product> affectedProducts, int productsToActivate, decimal discount)
            : base(id, affectedProducts, productsToActivate)
        {
            Discount = discount;
        }
        
        public decimal Discount { get; set; }
    }
}