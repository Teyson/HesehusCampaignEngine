namespace CampaignEngine.Domain
{
    public class CalculatedBasket
    {
        public CalculatedBasket(
            decimal total,
            HashSet<CampaignActivation> activatedCampaigns,
            List<Product> unaffectedProducts)
        {
            Total = total;
            ActivatedCampaigns = activatedCampaigns;
            UnaffectedProducts = unaffectedProducts;
        }

        public decimal Total { get; set; }
        
        public HashSet<CampaignActivation> ActivatedCampaigns { get; set; }
        
        public List<Product> UnaffectedProducts { get; set; }
    }
}