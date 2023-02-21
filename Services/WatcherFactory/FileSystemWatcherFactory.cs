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
    public class FileSystemWatcherFactory
    {
        public IFileSystemWatcherFactory GetFactory(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.TXT:
                    return new TxtFileSystemWatcherFactory();
                case FileType.CSV:
                    return new CsvFileSystemWatcherFactory();
                default:
                    throw new NotSupportedException($"File type '{fileType}' is not supported.");
            }
        }
    }
}
