using CampaignEngine.Domain;
using CampaignEngine.Domain.Campaigns;
using CampaignEngine.Domain.UndirectedGraph;

namespace CampaignEngine.Engine
{
    public class Engine
    {
        public HashSet<BasketActivation> BasketActivations = new();
        public List<CampaignActivation> CampaignActivations = new();
        public CalculatedBasket? CheapestBasket = null;

        /// <summary>
        /// Finds the cheapest possible combination of campaigns in the basket.
        /// </summary>
        /// <param name="basketLines">The basket lines in the basket</param>
        /// <param name="campaignsInBasket">The campaigns that affect at least one product in the basket</param>
        /// <returns>CalculatedBasket with the price</returns>
        public CalculatedBasket? CalculatePrice(
            List<OrderLine> basketLines,
            HashSet<Campaign> campaignsInBasket)
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

            CampaignActivations = GenerateCampaignActivations(basketProductsAffectedByCampaigns, campaignsInBasket)
                .OrderByDescending(x => x.AffectedProducts.Count)
                .ToList();

            var campaignOverlaps = GenerateCampaignOverlaps();

            if (CampaignActivations.Any())
                GenerateBasketActivations(0, new BasketActivation(), campaignOverlaps, products);

            return CheapestBasket;
        }

        private UndirectedGraph<CampaignActivation> GenerateCampaignOverlaps()
        {
            var graph = new UndirectedGraph<CampaignActivation>(CampaignActivations);

            for (var i = 0; i < CampaignActivations.Count; i++)
            for (var j = i + 1; j < CampaignActivations.Count; j++)
            {
                if (i == j)
                    continue;

                var campaignActivation1 = CampaignActivations[i];
                var campaignActivation2 = CampaignActivations[j];

                if (campaignActivation1.HasOverlap(campaignActivation2))
                    graph.AddEdge(CampaignActivations[i], CampaignActivations[j]);
            }

            return graph;
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
            var tempActivation = new Product[campaign.ProductsToActivate];

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
                var tempActivation2 = (Product[]) tempActivation.Clone();
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
            BasketActivation unfinishedBasketActivation,
            UndirectedGraph<CampaignActivation> campaignOverlaps,
            List<Product> products)
        {
            if (i >= CampaignActivations.Count)
            {
                unfinishedBasketActivation.UpdateUnaffectedProducts(products);
                unfinishedBasketActivation.UpdateTotal();
                
                if (CheapestBasket == null || CheapestBasket.Total < unfinishedBasketActivation.Total)
                    CheapestBasket = new CalculatedBasket(unfinishedBasketActivation.Total,
                        unfinishedBasketActivation.CampaignsInEffect, unfinishedBasketActivation.UnaffectedProducts);
            }
            else
            {
                var s = unfinishedBasketActivation.Clone();
                GenerateBasketActivations(i + 1, s, campaignOverlaps, products);

                if (!unfinishedBasketActivation.HasOverlap(CampaignActivations[i], campaignOverlaps))
                {
                    var s2 = unfinishedBasketActivation.Clone();
                    s2.CampaignsInEffect.Add(CampaignActivations[i]);
                    GenerateBasketActivations(i + 1, s2, campaignOverlaps, products);
                }
            }
        }

        private CalculatedBasket GetCheapestBasketActivation(List<Product> products)
        {
            BasketActivation cheapest = new() {Total = decimal.MaxValue};
            if (!BasketActivations.Any())
            {
                BasketActivations.Add(new BasketActivation(products, new HashSet<CampaignActivation>(), 0));
            }

            foreach (var basketActivation in BasketActivations)
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