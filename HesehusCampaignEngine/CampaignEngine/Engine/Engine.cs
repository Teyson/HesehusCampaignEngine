using CampaignEngine.Domain;
using CampaignEngine.Domain.Baskets;
using CampaignEngine.Domain.Campaigns;
using CampaignEngine.Domain.Products;
using CampaignEngine.Domain.UndirectedGraph;

namespace CampaignEngine.Engine
{
    public class Engine
    {
        public HashSet<BasketActivation> BasketActivations = new();
        public List<CampaignActivation> CampaignActivations = new();

        public CalculatedBasket CheapestBasket = new(int.MaxValue,
            new HashSet<CampaignActivation>(),
            new List<Product>());

        private CancellationToken _timeoutToken;

        /// <summary>
        /// Finds the cheapest possible combination of campaigns in the basket.
        /// </summary>
        /// <param name="basketLines">The basket lines in the basket</param>
        /// <param name="campaignsInBasket">The campaigns that affect at least one product in the basket</param>
        /// <param name="timeoutMs"><para>The max amount of time in ms the code can run.
        /// If timeout occours the engine returns the best price found in the time.
        /// It cannot be guaranteed that this is actually the best price.
        /// A timeout flag on the returned basket can be checked.</para><br/>
        /// 
        /// Suggested values:<br/>
        /// B2B solution with big and complex baskets: 500-1000 ms<br/>
        /// B2B solution with simple baskets: 250-500 ms<br/>
        /// B2C solution with big and complex baskets: 250-500 ms<br/>
        /// B2C solution with simple baskets: 100-250 ms<br/><br/>
        ///
        /// A value of -1 means no timeout</param>
        /// <returns>CalculatedBasket with the price</returns>
        public CalculatedBasket CalculatePrice(
            List<OrderLine> basketLines,
            HashSet<Campaign> campaignsInBasket,
            int timeoutMs)
        {
            using var cTSource = new CancellationTokenSource();
            if (timeoutMs == -1)
            {
                _timeoutToken = CancellationToken.None;
                cTSource.Dispose();
            }
            else
            {
                cTSource.CancelAfter(timeoutMs);
                _timeoutToken = cTSource.Token;
            }

            try
            {
                if (!basketLines.Any())
                    return new CalculatedBasket(0, new HashSet<CampaignActivation>(), new List<Product>());

                var products = basketLines.AsParallel().WithCancellation(_timeoutToken).SelectMany(OrderlineMapper.MapOrderlineToEnumerable).ToList();
                
                
                var campaignProductIds = campaignsInBasket
                    .AsParallel()
                    .WithCancellation(_timeoutToken)
                    .SelectMany(x => x.AffectedProducts.Select(product => product.Id))
                    .ToHashSet();

                var allProductsAffectedByCampaigns = products
                    .AsParallel()
                    .WithCancellation(_timeoutToken)
                    .Where(x => campaignProductIds.Contains(x.Id)).ToHashSet();

                CampaignActivations = GenerateCampaignActivations(allProductsAffectedByCampaigns, campaignsInBasket)
                    .AsParallel()
                    .WithCancellation(_timeoutToken)
                    .OrderByDescending(x => x.AffectedProducts.Count)
                    .ToList();

                var campaignOverlaps = GenerateCampaignOverlaps();
                
                if (CampaignActivations.Any())
                    GenerateBasketActivations(0, new BasketActivation(), campaignOverlaps, products);
                else
                    CheapestBasket = new CalculatedBasket(products.Sum(x => x.Price), 
                        new HashSet<CampaignActivation>(), 
                        new List<Product>());
                
                return CheapestBasket;
            }
            catch (OperationCanceledException e)
            {
                CheapestBasket.Timeout = true;
                return CheapestBasket;
            }
        }

        private UndirectedGraph<CampaignActivation> GenerateCampaignOverlaps()
        {
            var graph = new UndirectedGraph<CampaignActivation>(CampaignActivations);

            for (var i = 0; i < CampaignActivations.Count; i++)
            {
                _timeoutToken.ThrowIfCancellationRequested();
                for (var j = i + 1; j < CampaignActivations.Count; j++)
                {
                    if (i == j)
                        continue;

                    var campaignActivation1 = CampaignActivations[i];
                    var campaignActivation2 = CampaignActivations[j];

                    if (campaignActivation1.HasOverlap(campaignActivation2))
                        graph.AddEdge(CampaignActivations[i], CampaignActivations[j]);
                }
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
                _timeoutToken.ThrowIfCancellationRequested();

                var productsInCampaign = productsAffectedByCampaigns
                    .Where(x => campaign.AffectedProducts.Select(product => product.Id).Contains(x.Id))
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

                if (unfinishedBasketActivation.Total < CheapestBasket.Total)
                    CheapestBasket = new CalculatedBasket(unfinishedBasketActivation.Total,
                        unfinishedBasketActivation.CampaignsInEffect, unfinishedBasketActivation.UnaffectedProducts);

                _timeoutToken.ThrowIfCancellationRequested();
            }
            else
            {
                var s = unfinishedBasketActivation.Clone();
                GenerateBasketActivations(i + 1, s, campaignOverlaps, products);

                if (unfinishedBasketActivation.HasOverlap(CampaignActivations[i], campaignOverlaps))
                    return;

                var s2 = unfinishedBasketActivation.Clone();
                s2.CampaignsInEffect.Add(CampaignActivations[i]);
                GenerateBasketActivations(i + 1, s2, campaignOverlaps, products);
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