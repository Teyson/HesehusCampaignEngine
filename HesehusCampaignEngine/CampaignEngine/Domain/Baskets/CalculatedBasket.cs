using CampaignEngine.Domain.Campaigns;
using CampaignEngine.Domain.Products;

namespace CampaignEngine.Domain.Baskets
{
    public class CalculatedBasket
    {
        public CalculatedBasket(
            decimal total,
            HashSet<CampaignActivation> activatedCampaigns,
            List<Product> unaffectedProducts,
            bool timeout = false)
        {
            Total = total;
            ActivatedCampaigns = activatedCampaigns;
            UnaffectedProducts = unaffectedProducts;
            Timeout = timeout;
        }

        public decimal Total { get; set; }
        
        public HashSet<CampaignActivation> ActivatedCampaigns { get; set; }
        
        public List<Product> UnaffectedProducts { get; set; }

        public bool Timeout { get; set; }
    }
}