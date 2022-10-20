using System;
using System.Collections.Generic;

namespace HesehusCampaignEngine.Domain
{
    public class BasketActivation
    {
        public HashSet<Product> UnaffectedProducts { get; set; }
        public HashSet<CampaignActivation> CampaignsInEffect { get; set; }
        public decimal Total { get; set; }
        
        public BasketActivation(HashSet<Product> unaffectedProducts, HashSet<CampaignActivation> campaignsInEffect, decimal total)
        {
            UnaffectedProducts = unaffectedProducts;
            CampaignsInEffect = campaignsInEffect;
            Total = total;
        }

        private void UpdateUnaffectedProducts(List<Product> allProducts)
        {
            throw new NotImplementedException();
        }

        private void UpdateTotal()
        {
            throw new NotImplementedException();
        }
    }
}