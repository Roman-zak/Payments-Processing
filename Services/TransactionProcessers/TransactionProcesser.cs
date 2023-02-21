using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments_Processing
{
    internal class TransactionProcesser: ITransactionProcesser
    {
        
        private IParseStrategy parser;
        private IJsonWriter jsonWriter;

        public TransactionProcesser(IJsonWriter jsonWriter, string filePath)
        {
            this.FilePath = filePath;
            this.jsonWriter = jsonWriter;
        }
        public string FilePath { get; set; }
        public static int TodayFileNumber { get; set; }

        public void processFile()
        {
            mapParserToFile(FilePath);
            ISet<City> userTransactionsData = parser.parce(FilePath);

            string writeFilePath = GetWriteFilePath();
            
            jsonWriter.writeToJson(ref writeFilePath, userTransactionsData);
            TodayFileNumber++;
        }
        private string GetWriteFilePath()
        {
            string directory = FileDispatcher.WriteDirectory + "/" + DateTime.UtcNow.Date.ToString("MM-dd-yyyy") ;
            System.IO.Directory.CreateDirectory(directory);
            return  directory+"/"+ "output"+TodayFileNumber+".json";
        }
        public void setParser(IParseStrategy parseStrategy)
        {
           parser = parseStrategy;
        }
        private void mapParserToFile(string path)
        {
            string extension = Path.GetExtension(path);
            IParseStrategy parser = null;
            switch (extension)
            {
                case ".txt": parser = new TxtParser(); break;
                case ".csv": parser = new CsvParser(); break;
            }
            this.setParser(parser);
        }
    }
}
