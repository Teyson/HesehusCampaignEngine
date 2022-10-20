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

            switch (Campaign)
            {
                case PercentCampaign pC:
                    var sum = AffectedProducts.Sum(x => x.Price);
                    Total = sum - (sum * pC.DiscountPercentage / 100);
                    break;
                case DiscountCampaign dC:
                    var sum1 = AffectedProducts.Sum(x => x.Price);
                    Total = sum1 - dC.Discount;
                    break;
                case SetPriceCampaign sPC:
                    Total = sPC.NewTotalPrice;
                    break;
                default:
                    var sum2 = AffectedProducts.Sum(x => x.Price);
                    Total = sum2;
                    break;
            }
        }

        public Campaign Campaign { get; set; }
        public HashSet<Product> AffectedProducts { get; set; }
        public decimal Total { get; set; }
    }
}