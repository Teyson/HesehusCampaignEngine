using System.Collections.Generic;
using CampaignEngine.Domain;
using CampaignEngine.Domain.Campaigns;

namespace Test.Engine.Generators
{
    public class TestInstance
    {
        public IList<OrderLine> OrderLines { get; }
        public IList<Campaign> Campaigns { get; }

        public TestInstance(IList<OrderLine> orderLines, IList<Campaign> campaigns)
        {
            OrderLines = orderLines;
            Campaigns = campaigns;
        }
    }
}