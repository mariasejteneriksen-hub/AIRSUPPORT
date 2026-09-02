using CsvHelper;
using System.Globalization;
using AIRSUPPORT.Components.Models;

namespace AIRSUPPORT.Components.Services
{
    public class CompanyCleaningService
    {
        public void CleanCompanyData()
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                BadDataFound = null
            };

            using var reader = new StreamReader("wwwroot/Data/LIME Ekstrakt COMPANY.csv");
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            var rawRows = new List<dynamic>();
            while (csv.Read())
            {
                rawRows.Add(csv.GetRecord<dynamic>());
            }

            var cleaned = rawRows.Select(r =>
            {
                var row = (IDictionary<string, object>)r;

                return new CleanedCompany
                {
                    NavCustomerNo = row["Nav Customer No"]?.ToString(),
                    CompanyName = row["Company name"]?.ToString(),
                    CustomerStatus = row["Customer Status"]?.ToString(),
                    PriorityCustomer = CsvParsingHelper.ParseDouble(row["Priority Customer"]?.ToString()),
                    PPS = CsvParsingHelper.ParseDouble(row["Tails – PPS"]?.ToString()?.Replace(",",".")),
                    OcFwTerr = CsvParsingHelper.ParseDouble(row["Tails – OC-FW, Terr"]?.ToString()?.Replace(",", ".")),
                    OcFwSat = CsvParsingHelper.ParseDouble(row["Tails – OC-FW, SAT"]?.ToString()?.Replace(",", ".")),
                    OcNotam = CsvParsingHelper.ParseDouble(row["Tails - OC-NOTAM"]?.ToString()?.Replace(",", ".")),
                    CustomerSince = CsvParsingHelper.ParseDate(row["Customer Since"]?.ToString()),
                    PriceEscalation = CsvParsingHelper.ParseDecimal(row["Price escalation %"]?.ToString()?.Replace(",", "."))
                };
            }).ToList();

            using var writer = new StreamWriter("wwwroot/Data/company_cleaned.csv");
            using var csvWriter = new CsvWriter(writer, config);
            csvWriter.WriteRecords(cleaned);
        }

        private string? ExtractCountryCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var parts = value.Split('-', StringSplitOptions.TrimEntries);
            return parts.Length >= 2 ? parts[1] : value.Trim();
        }
    }
}