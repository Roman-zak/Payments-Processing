using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Payments_Processing
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                            new FileDispatcher()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
