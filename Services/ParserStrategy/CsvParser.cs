using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments_Processing
{
    internal class CsvParser : IParseStrategy
    {
        ISet<City> IParseStrategy.parce(string file_path)
        {
            var cities = new HashSet<City>();

            using (var reader = new StreamReader(file_path))
            {
                City currentCity = null;
                Service currentService = null;
                bool firstLine = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    var values = line.Split(',').Select(v => v.Trim()).ToList();

                    if (values.Count != 9)
                    {
                        FileDispatcher.addInvalidFile(file_path);
                        FileDispatcher.incrementFoundErrorsCount();
                        continue;
                    }
                    string firstName;
                    string lastName;
                    string city;
                    decimal payment;
                    DateTime date;
                    long accountNumber;
                    string service;
                    try
                    {
                        firstName = values[0];
                        lastName = values[1];
                        city = values[2].Substring(1, values[2].Length - 1);
                        payment = decimal.Parse(values[5], CultureInfo.InvariantCulture);
                        date = DateTime.ParseExact(values[6], "yyyy-dd-MM", null, System.Globalization.DateTimeStyles.None);
                        accountNumber = long.Parse(values[7]);
                        service = values[8];
                    }
                    catch (FormatException ex)
                    {
                        FileDispatcher.addInvalidFile(file_path);
                        FileDispatcher.incrementFoundErrorsCount();
                        continue;
                    }


                    if (currentCity == null || currentCity.Name != city)
                    {
                        var tempCity = new City { Name = city };
                        if (cities.Contains(tempCity))
                        {
                            currentCity = cities.First(c => c.Name == tempCity.Name);
                        }
                        else
                        {
                            currentCity = tempCity;
                            cities.Add(currentCity);
                            currentService = null;
                        }
                    }

                    if (currentService == null || currentService.Name != service)
                    {
                        var tempService = new Service { Name = service };
                        if (currentCity.Services.Contains(tempService))
                        {
                            currentService = currentCity.Services.First(c => c.Name == tempService.Name);
                        }
                        else
                        {
                            currentService = tempService;
                            currentCity.Services.Add(currentService);
                        }
                    }

                    currentService.Payers.Add(new Payer
                    {
                        Name = $"{firstName} {lastName}",
                        Payment = payment,
                        Date = date,
                        AccountNumber = accountNumber
                    });
                    FileDispatcher.incrementParsedLinesCount();
                }
            }
            FileDispatcher.incrementParsedFilesCount();
            return cities;
        }
    }
}
