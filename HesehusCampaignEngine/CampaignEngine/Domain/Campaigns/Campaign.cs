using System.Collections.Generic;

namespace CampaignEngine.Domain.Campaigns
{
    public abstract class Campaign
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

        public abstract decimal CalculatePrice(HashSet<Product> activatedProducts);
        
        public override string ToString()
        {
            return $"Id: {Id} || ProductsToActivate: {ProductsToActivate} || Type: {this.GetType()}";
        }
    }
}