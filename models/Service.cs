using System.Collections.Generic;
using System.Linq;
namespace Payments_Processing
{
    public class Service
    {
        public string Name { get; set; }
        public List<Payer> Payers { get; set; } = new List<Payer>();
        public decimal Total => Payers.Sum(p => p.Payment);
    }
}