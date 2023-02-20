using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Payments_Processing
{
    internal class TransactionJsonWriter : IJsonWriter
    {
        public void writeToJson(ref string jsonPath, List<City> userTransactionsData)
        {
            var json = JsonConvert.SerializeObject(userTransactionsData, Formatting.Indented);
            File.WriteAllText(jsonPath, json); ;
        }
    }
}
