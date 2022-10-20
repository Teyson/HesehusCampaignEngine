using System.Collections.Generic;
using HesehusCampaignEngine.Domain.Campaigns;

namespace HesehusCampaignEngine.Domain
{
    public class CampaignActivation
    {
        public CampaignActivation(Campaign campaign, HashSet<Product> affectedProducts, decimal total)
        {
            Campaign = campaign;
            AffectedProducts = affectedProducts;
            Total = total;
        }

        public Campaign Campaign { get; set; }
        public HashSet<Product> AffectedProducts { get; set; }
        public decimal Total { get; set; }
    }
}