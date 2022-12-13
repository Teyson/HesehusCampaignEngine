using CampaignEngine.Domain.Products;

namespace CampaignEngine.Domain.Campaigns
{
    public class CampaignActivation
    {
        public CampaignActivation(Campaign campaign, HashSet<Product> affectedProducts)
        {
            Campaign = campaign;
            AffectedProducts = affectedProducts;
            Total = Campaign.CalculatePrice(affectedProducts);
        }

        public Campaign Campaign { get; set; }
        public HashSet<Product> AffectedProducts { get; set; }
        public decimal Total { get; set; }

        public bool Contains(Product p)
        {
            return AffectedProducts.Contains(p);
        }

        public bool HasOverlap(CampaignActivation other)
        {
            return other.AffectedProducts.Any(Contains);
        }
    }
}