namespace HesehusCampaignEngine.Domain
{
    public class CalculatedBasket
    {
        public CalculatedBasket(decimal total)
        {
            Total = total;
        }

        public decimal Total { get; set; }
    }
}