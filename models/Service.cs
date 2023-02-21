using System.Collections.Generic;
using System.Linq;
namespace Payments_Processing
{
    public class Service
    {
        public string Name { get; set; }
        public List<Payer> Payers { get; set; } = new List<Payer>();
        public decimal Total => Payers.Sum(p => p.Payment);

        public override bool Equals(object obj)
        {
            return obj is Service service &&
                   Name == service.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}