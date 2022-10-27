using System;
using System.Linq;

namespace CampaignEngine.Domain
{
    public class Product : IEquatable<Product>

    {
        public Product(string id, decimal price, int subId = 0)
        {
            Id = id;
            Price = price;
            SubId = subId;
        }

        public string Id { get; set; }
        public int SubId { get; set; }
        public decimal Price { get; set; }


        public bool Equals(Product? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Price == other.Price && SubId == other.SubId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Price);
        }
    }
}