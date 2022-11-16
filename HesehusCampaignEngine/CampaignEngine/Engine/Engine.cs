using System.Collections.Generic;
using System.Linq;
using CampaignEngine.Domain;
using CampaignEngine.Domain.Campaigns;

namespace CampaignEngine.Engine
{
    public class Engine
    {
        public HashSet<BasketActivation> _basketActivations = new();
        public List<CampaignActivation> _campaignActivations = new();

        /// <summary>
        /// Finds the cheapest possible combination of campaigns in the basket.
        /// </summary>
        /// <param name="basketLines">The basket lines in the basket</param>
        /// <param name="campaignsInBasket">The campaigns that affect at least one product in the basket</param>
        /// <returns>CalculatedBasket with the price</returns>
        public CalculatedBasket CalculatePrice(List<OrderLine> basketLines, HashSet<Campaign> campaignsInBasket)
        {
            if (!basketLines.Any())
                return new CalculatedBasket(0, new HashSet<CampaignActivation>(), new List<Product>());

            var products = basketLines.SelectMany(orderLine =>
            {
                List<Product> products = new();
                for (var i = 0; i < orderLine.Amount; i++)
                {
                    products.Add(new Product(orderLine.Product.Id, orderLine.Product.Price, i));
                }

                return products;
            }).ToList();

            var allProductsAffectedByCampaigns = campaignsInBasket
                .SelectMany(x => x.AffectedProducts)
                .ToHashSet();

            var basketProductsOutsideCampaigns = products.Except(allProductsAffectedByCampaigns);

            var basketProductsAffectedByCampaigns = products.Except(basketProductsOutsideCampaigns).ToHashSet();

            _campaignActivations = GenerateCampaignActivations(basketProductsAffectedByCampaigns, campaignsInBasket);

            if (_campaignActivations.Any())
                GenerateBasketActivations(0, new BasketActivation());

            var cheapestBasket = GetCheapestBasketActivation(products);

            return cheapestBasket;
        }

        private List<CampaignActivation> GenerateCampaignActivations(
            HashSet<Product> productsAffectedByCampaigns,
            HashSet<Campaign> campaigns)
        {
            List<CampaignActivation> allCampaignActivations = new();

            foreach (var campaign in campaigns)
            {
                var productsInCampaign = productsAffectedByCampaigns
                    .Where(x => campaign.AffectedProducts.Contains(x))
                    .ToList();

                var combinationsForCampaign = GenerateCampaignActivationsForCampaign(
                    productsInCampaign,
                    campaign);

                allCampaignActivations.AddRange(combinationsForCampaign);
            }

            return allCampaignActivations;
        }

        private HashSet<CampaignActivation> GenerateCampaignActivationsForCampaign(
            List<Product> productsHitByCampaign,
            Campaign campaign)
        {
            Product[] tempActivation = new Product[campaign.ProductsToActivate];

            List<Product[]> activations = new();

            GenerateCampaignActivation(
                productsHitByCampaign,
                tempActivation,
                activations,
                0,
                productsHitByCampaign.Count - 1,
                0,
                campaign.ProductsToActivate);

            var campaignActivations = new HashSet<CampaignActivation>();

            foreach (var activation in activations)
            {
                var campaignActivation = new CampaignActivation(campaign, activation.ToHashSet());
                campaignActivations.Add(campaignActivation);
            }

            return campaignActivations;
        }

        private void GenerateCampaignActivation(
            List<Product> productsHitByCampaign,
            Product[] tempActivation,
            List<Product[]> activations,
            int start,
            int end,
            int index,
            int campaignProductsToActivate)
        {
            if (index >= campaignProductsToActivate)
            {
                activations.Add(tempActivation);
                return;
            }

            for (var i = start; i <= end && end - i + 1 >= campaignProductsToActivate - index; i++)
            {
                tempActivation[index] = productsHitByCampaign[i];
                var tempActivation2 = (Product[])tempActivation.Clone();
                GenerateCampaignActivation(
                    productsHitByCampaign,
                    tempActivation2,
                    activations,
                    i + 1,
                    end,
                    index + 1,
                    campaignProductsToActivate);
            }
        }


        private void GenerateBasketActivations(
            int i,
            BasketActivation unfinishedBasketActivation)
        {
            if (i >= _campaignActivations.Count)
                _basketActivations.Add(unfinishedBasketActivation);
            else
            {
                var s = unfinishedBasketActivation.Clone();
                GenerateBasketActivations(i + 1, s);
                var s2 = unfinishedBasketActivation.Clone();
                s2.CampaignsInEffect.Add(_campaignActivations[i]);
                GenerateBasketActivations(i + 1, s2);
            }
        }

        private CalculatedBasket GetCheapestBasketActivation(List<Product> products)
        {
            BasketActivation cheapest = new() {Total = decimal.MaxValue};
            if (!_basketActivations.Any())
            {
                _basketActivations.Add(new BasketActivation(products, new HashSet<CampaignActivation>(), 0));
            }

            foreach (var basketActivation in _basketActivations)
            {
                var valid = basketActivation.IsValid();

                if (!valid) continue;

                basketActivation.UpdateUnaffectedProducts(products);
                basketActivation.UpdateTotal();
                if (basketActivation.Total < cheapest.Total)
                    cheapest = basketActivation;
            }

            return new CalculatedBasket(cheapest.Total, cheapest.CampaignsInEffect, cheapest.UnaffectedProducts);
        }
    }
}