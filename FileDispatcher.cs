using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using Payments_Processing.watchers;
using Payments_Processing.models;

namespace Payments_Processing
{
    [RunInstaller(true)]
    public partial class FileDispatcher : ServiceBase
    {
        private static ISet<FileSystemWatcher> watchers;
        private static object _lock = new object();
        private Timer timer;
        public static string WriteDirectory { get; set; }
        public string ReadDirectory { get; private set; }

        private static int _parsedFilesCount;

        private static int _parsedLinesCount;

        private static int _foundErrorsCount;

        private static ISet<string> _invalidFiles;
        public FileDispatcher()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteDirectory = ConfigurationManager.AppSettings["writeDirectory"];
            ReadDirectory = ConfigurationManager.AppSettings["readDirectory"];
            FileSystemWatcherFactory fileSystemWatcherFactory = new FileSystemWatcherFactory();

            watchers = new HashSet<FileSystemWatcher>();
            watchers.Add(fileSystemWatcherFactory.GetFactory(FileType.TXT).Create(ReadDirectory));
            watchers.Add(fileSystemWatcherFactory.GetFactory(FileType.CSV).Create(ReadDirectory));

            _invalidFiles=new HashSet<string>();

            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan dueTime = TimeSpan.FromHours(24) - now; // calculate time until next midnight
            timer = new Timer(CreateMetaFile, null, (int)dueTime.TotalMilliseconds, (int)TimeSpan.FromHours(24).TotalMilliseconds);

        }

        protected override void OnStop()
        {
            watchers.ToList().ForEach(w => w.Dispose());

            timer.Dispose();
        }

        internal static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string newFilePath = e.FullPath;

            TransactionProcesser processer = new TransactionProcesser(new TransactionJsonWriter(), newFilePath);

            Thread thread = new Thread(new ThreadStart(processer.processFile));

            thread.Start();
        }
        internal static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
        private static void CreateMetaFile(object source)
        {
            string metaFilePath = GetMetaFilePath();

            using (StreamWriter writer = new StreamWriter(metaFilePath))
            {
                writer.WriteLine("parsed_files: " + _parsedFilesCount);
                writer.WriteLine("parsed_lines: " + _parsedLinesCount);
                writer.WriteLine("found_errors: " + _foundErrorsCount);
                writer.Write("invalid_files: [");
                writer.Write(String.Join(", ", _invalidFiles));
                writer.Write("]");
            }
            resetMetadataCounters();
        }

        private static void resetMetadataCounters()
        {
            lock (_lock)
            {
                _parsedFilesCount = 0;
                _parsedLinesCount = 0;
                _foundErrorsCount = 0;
                _invalidFiles.Clear();
            }
        }

        private static string GetMetaFilePath()
        {
            string directory = WriteDirectory + "/" + DateTime.UtcNow.Date.ToString("MM-dd-yyyy");
            System.IO.Directory.CreateDirectory(directory);
            return directory+"/meta.log" ;
        }
        public static void incrementParsedFilesCount()
        {
            lock (_lock)
            {
                _parsedFilesCount++;
            }
        }
        public static void incrementParsedLinesCount()
        {
            lock (_lock)
            {
                _parsedLinesCount++;
            }
        }
        public static void incrementFoundErrorsCount()
        {
            lock (_lock)
            {
                _foundErrorsCount++;
            }
        }
        public static void addInvalidFile(string invalidFilePath)
        {
            lock (_lock)
            {
                _invalidFiles.Add(invalidFilePath);
            }
        }
    }
}
