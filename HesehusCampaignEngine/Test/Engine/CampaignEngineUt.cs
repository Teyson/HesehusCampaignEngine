using System.Collections.Generic;
using CampaignEngine.Domain;
using CampaignEngine.Domain.Campaigns;
using NUnit.Framework;

namespace Test.Engine
{
    [TestFixture]
    public class CampaignEngineUt
    {
        [Test]
        public void CalculatePrice_Returns_CorrectPrice_LowStress()
        {
            //Arrange
            Product p1 = new Product("1", 200);
            Product p2 = new Product("2", 100);

            List<OrderLine> basketLines = new() {new OrderLine(p1, 1), new OrderLine(p2, 1)};

            //Act
            var calculatedBasket =
                CampaignEngine.Engine.CampaignEngine.CalculatePrice(basketLines, new HashSet<Campaign>());

            //Assert
            Assert.That(calculatedBasket.Total, Is.EqualTo(300));
        }

        [Test]
        public void CalculatePrice_Returns_CorrectPrice_AverageStress()
        {
            //Arrange
            Product p1 = new Product("1", 200);
            Product p2 = new Product("2", 100);
            Product p3 = new Product("3", 300);

            List<OrderLine> basketLines = new() {new OrderLine(p1, 1), new OrderLine(p2, 1)};

            Campaign c1 = new DiscountCampaign("c1", new HashSet<Product>() {p1}, 1, 50);
            Campaign c2 = new DiscountCampaign("c2", new HashSet<Product>() {p2}, 1, 50);
            Campaign c3 = new DiscountCampaign("c3", new HashSet<Product>() {p3}, 1, 200);

            HashSet<Campaign> campaigns = new(){c1, c2, c3};

            //Act
            var calculatedBasket = 
                CampaignEngine.Engine.CampaignEngine.CalculatePrice(basketLines, new HashSet<Campaign>());

            //Assert
            Assert.That(calculatedBasket.Total, Is.EqualTo(300));
        }
        
        [Test]
        public void CalculatePrice_Returns_CorrectPrice_HighStress()
        {
            //Arrange
            Product p1 = new Product("1", 100);
            Product p2 = new Product("2", 100);
            Product p3 = new Product("3", 100);
            Product p4 = new Product("4", 100);
            Product p5 = new Product("5", 100);
            Product p6 = new Product("6", 100);
            Product p7 = new Product("7", 100);
            Product p8 = new Product("8", 100);
            Product p9 = new Product("9", 100);
            Product p10 = new Product("10", 100);

            List<OrderLine> basketLines = new()
            {
                new OrderLine(p1, 1),
                new OrderLine(p2, 1),
                new OrderLine(p3, 1),
                new OrderLine(p4, 1),
                new OrderLine(p5, 1),
                new OrderLine(p6, 1),
                new OrderLine(p7, 1),
                new OrderLine(p8, 1),
                new OrderLine(p9, 1),
                new OrderLine(p10, 1),
            };

            Campaign c1 = new PercentCampaign("c1", new HashSet<Product>() {p1, p2, p3}, 2, 50);
            Campaign c2 = new DiscountCampaign("c2", new HashSet<Product>() {p1}, 1, 50);
            Campaign c3 = new SetPriceCampaign("c3", new HashSet<Product>() {p3, p4}, 2, 75);
            Campaign c4 = new PercentCampaign("c4", new HashSet<Product>() {p5}, 1, 10);

            HashSet<Campaign> campaigns = new(){c1, c2, c3, c4};

            //Act
            var calculatedBasket = 
                CampaignEngine.Engine.CampaignEngine.CalculatePrice(basketLines, new HashSet<Campaign>());

            //Assert
            Assert.That(calculatedBasket.Total, Is.EqualTo(765));
        }
    }
}