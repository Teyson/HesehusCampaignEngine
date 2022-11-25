namespace CampaignEngine.Domain.Products
{
    public class OrderLine
    {
        public OrderLine(Product product, int amount)
        {
            Product = product;
            Amount = amount;
        }

        public Product Product { get; set; }
        public int Amount { get; set; }
    }
}