using System.Collections.Generic;
using System.IO;

namespace Payments_Processing
{
    public interface IParseStrategy
    {
        List<City> parce(string file_path); 
    }
}