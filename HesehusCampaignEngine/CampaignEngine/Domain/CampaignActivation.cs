using System.Collections.Generic;
using System.Linq;
using CampaignEngine.Domain.Campaigns;

namespace CampaignEngine.Domain
{
    public class CampaignActivation
    {
        public CampaignActivation(Campaign campaign, HashSet<Product> affectedProducts)
        {
            Campaign = campaign;
            AffectedProducts = affectedProducts;
            Total = campaign.CalculatePrice(affectedProducts);
        }

        public Campaign Campaign { get; set; }
        public HashSet<Product> AffectedProducts { get; set; }
        public decimal Total { get; set; }

        public bool Contains(Product p)
        {
            return AffectedProducts.Contains(p);
        }
    }
}