namespace HesehusCampaignEngine.Domain
{
    public class Product
    {
        public Product(string id, decimal price)
        {
            Id = id;
            Price = price;
        }

        public string Id { get; set; }
        public decimal Price { get; set; }
    }
}