using System.Collections.Generic;
using CampaignEngine.Domain;
using CampaignEngine.Domain.Activations;
using CampaignEngine.Domain.Campaigns;
using CampaignEngine.Domain.Products;
using NUnit.Framework;

namespace Test.Domain.Campaigns
{
    [TestFixture]
    public class CampaignUt
    {
        [Test]
        public void PercentCampaign_CalculatesCorrect()
        {
            //Arrange
            Product p1 = new("1", 100);

            PercentCampaign c1 = new("A", new HashSet<Product>() {p1}, 1, 10);
            
            //Act
            CampaignActivation ca1 = new(c1, new HashSet<Product>() {p1});

            //Assert
            Assert.That(ca1.Total, Is.EqualTo(90));
        }
        
        [Test]
        public void DiscountCampaign_CalculatesCorrect()
        {
            //Arrange
            Product p1 = new("1", 100);
            Product p2 = new("2", 100);

            DiscountCampaign c1 = new("A", new HashSet<Product>() {p1, p2}, 1, 50);
            
            //Act
            CampaignActivation CA1 = new(c1, new HashSet<Product>() {p1});

            //Assert
            Assert.That(CA1.Total, Is.EqualTo(50));
        }
        
        [Test]
        public void SetPriceCampaign_CalculatesCorrect()
        {
            //Arrange
            Product p1 = new("1", 100);

            SetPriceCampaign c1 = new("A", new HashSet<Product>() {p1}, 1, 5000);
            
            //Act
            CampaignActivation CA1 = new(c1, new HashSet<Product>() {p1});

            //Assert
            Assert.That(CA1.Total, Is.EqualTo(5000));
        }
    }
}