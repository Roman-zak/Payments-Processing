using Payments_Processing.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments_Processing.watchers
{

    public class CsvFileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        public FileSystemWatcher Create(string path)
        {
            var watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
            watcher.Created += FileDispatcher.OnCreated;
            watcher.Error += FileDispatcher.OnError;
            watcher.Filter = "*.csv";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            return watcher;
        }
    }
}
