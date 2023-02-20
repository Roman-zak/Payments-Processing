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


namespace Payments_Processing
{
    [RunInstaller(true)]
    public partial class FileDispatcher : ServiceBase, ITransactionProcesser
    {
        private static FileSystemWatcher Watcher;
        private static IParseStrategy Parser;
        private static IJsonWriter JsonWriter;
        private static string WriteDirectory;
        private static uint todayFileNumber;

        public FileDispatcher()
        {
            InitializeComponent();
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            string command = "";
            do
            {
                Console.WriteLine("To stop service - type STOP");
                command = Console.ReadLine();
            } while (command != "STOP");
            this.OnStop();
        }
        protected override void OnStart(string[] args)
        {
            Watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["readDirectory"]);
            WriteDirectory = ConfigurationManager.AppSettings["writeDirectory"];
            todayFileNumber = 0;

            Watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            Watcher.Changed += OnChanged;
            Watcher.Created += OnCreated;
            Watcher.Deleted += OnDeleted;
            Watcher.Renamed += OnRenamed;
            Watcher.Error += OnError;

            Watcher.Filter = "*.txt";
            Watcher.IncludeSubdirectories = true;
            Watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            Watcher.Dispose();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("created");
            string value = $"Created: {e.FullPath}";
            UserTransactionsData userTransactionsData = Parser.parce(e.FullPath);
            string writeFilePath=WriteDirectory + DateTime.UtcNow.Date.ToString("dd-MM-yyyy")+todayFileNumber;
            JsonWriter.writeToJson(ref writeFilePath, userTransactionsData); 
            File.WriteAllText("WriteText.txt", value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
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

        public void setParser(IParseStrategy parseStrategy)
        {
           Parser = parseStrategy;
        }
    }
}
