using CampaignEngine.Domain.Products;

namespace CampaignEngine.Domain.Activations
{
    public class BasketActivation
    {
        public List<Product> UnaffectedProducts { get; set; }
        public HashSet<CampaignActivation> CampaignsInEffect { get; set; }
        public decimal Total { get; set; }

        public BasketActivation()
        {
            UnaffectedProducts = new List<Product>();
            CampaignsInEffect = new HashSet<CampaignActivation>();
            Total = 0;
        }
        
        public BasketActivation(List<Product> unaffectedProducts, HashSet<CampaignActivation> campaignsInEffect, decimal total)
        {
            UnaffectedProducts = unaffectedProducts;
            CampaignsInEffect = campaignsInEffect;
            Total = total;
        }

        public void UpdateUnaffectedProducts(List<Product> allProducts)
        {
            UnaffectedProducts = allProducts
                .Where(product => !CampaignsInEffect.Any(campaign => campaign.Contains(product)))
                .ToList();
        }

        public void UpdateTotal()
        {
            var total = UnaffectedProducts.Sum(product => product.Price);

            total += CampaignsInEffect.Sum(campaignActivation => campaignActivation.Total);

            Total = total;
        }

        public BasketActivation Clone()
        {
            return new BasketActivation(new List<Product>(UnaffectedProducts),
                new HashSet<CampaignActivation>(CampaignsInEffect), Total);
        }

        public bool IsValid()
        {
            var valid = true;
            foreach (var campaignActivation in CampaignsInEffect)
            {
                foreach (var otherActivation in CampaignsInEffect)
                {
                    if (campaignActivation == otherActivation)
                        continue;
                    
                    valid = !campaignActivation.AffectedProducts.Overlaps(otherActivation.AffectedProducts);
                    if (!valid)
                        return valid;
                }
            }

            return valid;
        }
    }
}