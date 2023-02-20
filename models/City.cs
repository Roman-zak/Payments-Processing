using System.Collections.Generic;
using System.Linq;
namespace Payments_Processing
{
    public class City
    {
        public string Name { get; set; }
        public List<Service> Services { get; set; } = new List<Service>();
        public decimal Total => Services.Sum(s => s.Total);
    }
}