using System.Collections.Generic;

namespace CampaignEngine.Domain.Campaigns
{
    public class SetPriceCampaign : Campaign
    {
        public SetPriceCampaign(string id, HashSet<Product> affectedProducts, int productsToActivate, decimal newTotalPrice)
            : base(id, affectedProducts, productsToActivate)
        {
            NewTotalPrice = newTotalPrice;
        }
        
        public decimal NewTotalPrice { get; set; }
    }
}