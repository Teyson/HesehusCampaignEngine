using System.Collections.Generic;
using CampaignEngine.Domain.Products;

namespace CampaignEngine.Domain.Campaigns
{
    public class SetPriceCampaign : Campaign
    {
        public SetPriceCampaign(string id, HashSet<Product> affectedProducts, int productsToActivate, decimal newTotalPrice)
            : base(id, affectedProducts, productsToActivate)
        {
            NewTotalPrice = newTotalPrice;
        }
        
        public override decimal CalculatePrice(HashSet<Product> activatedProducts)
        {
            return NewTotalPrice;
        }
        
        public decimal NewTotalPrice { get; set; }
    }
}