using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments_Processing.watchers
{
    public interface IFileSystemWatcherFactory
    {
        FileSystemWatcher Create(string path);
    }
}
