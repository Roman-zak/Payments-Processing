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

namespace Payments_Processing
{
    [RunInstaller(true)]
    public partial class FileDispatcher : ServiceBase
    {
        private static Dictionary<string, FileSystemWatcher> watchers;
        public string ReadDirectory { get; set; }

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
            ReadDirectory = ConfigurationManager.AppSettings["readDirectory"];

            watchers = new Dictionary<string, FileSystemWatcher>();

            watchers.Add("*.txt", new FileSystemWatcher(ConfigurationManager.AppSettings["readDirectory"]));
            watchers.Add("*.cvv", new FileSystemWatcher(ConfigurationManager.AppSettings["readDirectory"]));

            watchers.Select(w=> {
                w.Value.NotifyFilter =NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
                w.Value.Created += OnCreated;
                w.Value.Error += OnError;
                w.Value.Filter = w.Key;
                w.Value.IncludeSubdirectories = true;
                w.Value.EnableRaisingEvents = true;

                return w;
                }).ToDictionary(w => w.Key, w => w.Value);
            
        }

        protected override void OnStop()
        {
            watchers.Values.ToList().ForEach(w => w.Dispose());
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("created");
            string newFilePath = e.FullPath;
            TransactionProcesser processer = new TransactionProcesser(newFilePath);
            Thread thread = new Thread(new ThreadStart(processer.processFile));
            thread.Start();
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


    }
}
