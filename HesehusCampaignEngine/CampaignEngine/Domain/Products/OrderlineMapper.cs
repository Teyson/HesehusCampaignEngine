namespace CampaignEngine.Domain.Products;

public static class OrderlineMapper
{
    public static IEnumerable<Product> MapOrderlineToEnumerable(OrderLine orderLine)
    {
        List<Product> products = new();
        for (var i = 0; i < orderLine.Amount; i++)
        {
            products.Add(new Product(orderLine.Product.Id, orderLine.Product.Price, i));
        }

        return products;
    }
}