using System.Collections.Generic;

namespace HesehusCampaignEngine.Domain.Campaigns
{
    public class Campaign
    {
        public string Id { get; set; }
        public HashSet<Product> AffectedProducts { get; set; }
        public int ProductsToActivate { get; set; }
        
        public Campaign(string id, HashSet<Product> affectedProducts, int productsToActivate)
        {
            Id = id;
            AffectedProducts = affectedProducts;
            ProductsToActivate = productsToActivate;
        }
    }
}