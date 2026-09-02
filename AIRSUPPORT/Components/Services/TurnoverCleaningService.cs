using CsvHelper;
using System.Globalization;
using AIRSUPPORT.Components.Models;

namespace AIRSUPPORT.Components.Services
{
    public class TurnoverCleaningService
    {
        public void CleanTurnoverData()
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                BadDataFound = null
            };

            using var reader = new StreamReader("wwwroot/Data/LIME Ekstrakt Turnover.csv");
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

                var value2022 = CsvParsingHelper.ParseDouble(row[" 2 022"]?.ToString());
                var value2025 = CsvParsingHelper.ParseDouble(row["2,025.00"]?.ToString());

                return new CleanedTurnover
                {
                    NavCustomerNo = row["Company.Nav Customer No"]?.ToString() ?? "",
                    CompanyName = row["Company.Company name"]?.ToString() ?? "",
                    Country = ExtractCountryCode(row["Company.Country"]?.ToString()),
                    Last12Months = CsvParsingHelper.ParseDouble(row["Last 12 months"]?.ToString()),
                    Balance = CsvParsingHelper.ParseDouble(row["Balance"]?.ToString()),
                    FleetSize = CsvParsingHelper.ParseInt(row["Company.Fleet Size"]?.ToString()),
                    Year2022 = value2022,
                    Dev2223 = CsvParsingHelper.ParseDouble(row["Dev22-23"]?.ToString()),
                    Year2023 = CsvParsingHelper.ParseDouble(row[" 2 023"]?.ToString()),
                    Dev2324 = CsvParsingHelper.ParseDouble(row["Dev23-24"]?.ToString()),
                    Year2024 = CsvParsingHelper.ParseDouble(row[" 2 024"]?.ToString()),
                    Dev2425 = CsvParsingHelper.ParseDouble(row["Dev24-25"]?.ToString()),
                    Year2025 = value2025,
                    YearToDate = CsvParsingHelper.ParseDouble(row["Year to date"]?.ToString()),
                    LastYearToDate = CsvParsingHelper.ParseDouble(row["Last year to date"]?.ToString()),
                    LastFinancialYear = CsvParsingHelper.ParseDouble(row["Last financial year"]?.ToString())
                };
            }).ToList();

            using var writer = new StreamWriter("wwwroot/Data/turnover_cleaned.csv");
            using var csvWriter = new CsvWriter(writer, config);
            csvWriter.WriteRecords(cleaned);
        }

        private string ExtractCountryCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var parts = value.Split('-', StringSplitOptions.TrimEntries);

            if (parts.Length >= 2)
                return parts[1]; // landekoden i midten, fx "BE"

            return value.Trim();
        }

        private double? CalculateGrowthRate(double value2022, double value2025)
        {
            if (value2022 == 0)
                return null; // undgå division med 0

            return (value2025 - value2022) / value2022 * 100;
        }
    }
}