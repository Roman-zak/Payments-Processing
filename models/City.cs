using System.Collections.Generic;
using System.Linq;
namespace Payments_Processing
{
    public class City
    {
        public string Name { get; set; }
        public List<Service> Services { get; set; } = new List<Service>();
        public decimal Total => Services.Sum(s => s.Total);

        public override bool Equals(object obj)
        {
            return obj is City city &&
                   Name == city.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}