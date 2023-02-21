using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments_Processing
{
    internal interface ITransactionProcesser
    {
        void setParser(IParseStrategy parseStrategy);
    }
}
