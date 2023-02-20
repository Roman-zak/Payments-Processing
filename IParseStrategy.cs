using System.IO;

namespace Payments_Processing
{
    public interface IParseStrategy
    {
        UserTransactionsData parce(string file_path); 
    }
}