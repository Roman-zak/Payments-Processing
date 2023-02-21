using System.Collections.Generic;
using System.IO;

namespace Payments_Processing
{
    public interface IParseStrategy
    {
        ISet<City> parce(string file_path); 
    }
}