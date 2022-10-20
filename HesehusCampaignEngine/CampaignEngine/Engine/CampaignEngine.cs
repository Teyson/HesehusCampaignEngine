using System;
using System.Collections.Generic;
using HesehusCampaignEngine.Domain;
using HesehusCampaignEngine.Domain.Campaigns;

namespace HesehusCampaignEngine.Engine
{
    public class CampaignEngine
    {
        /// <summary>
        /// Finds the cheapest possible combination of campaigns in the basket.
        /// </summary>
        /// <param name="basketLines">The basket lines in the basket</param>
        /// <param name="campaignsInBasket">The campaigns that affect at least one product in the basket</param>
        /// <returns>CalculatedBasket with the price</returns>
        public static CalculatedBasket CalculatePrice(List<OrderLine> basketLines, HashSet<Campaign> campaignsInBasket)
        {
            throw new NotImplementedException();
        }

        private static HashSet<CampaignActivation> GenerateCampaignActivations(
            HashSet<Product> basket,
            HashSet<Campaign> campaigns)
        {
            throw new NotImplementedException();
        }

        private static HashSet<BasketActivation> GenerateBasketActivations(
            int i,
            BasketActivation unfinishedBasketActivation)
        {
            throw new NotImplementedException();
        }

        private static CalculatedBasket GetCheapestBasketActivation(HashSet<BasketActivation> basketActivations)
        {
            throw new NotImplementedException();
        }
    }
}