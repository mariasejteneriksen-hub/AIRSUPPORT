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

                var value2022 = ParseDouble(row[" 2 022"]?.ToString());
                var value2025 = ParseDouble(row["2,025.00"]?.ToString());

                return new CleanedTurnover
                {
                    NavCustomerNo = row["Company.Nav Customer No"]?.ToString() ?? "",
                    CompanyName = row["Company"]?.ToString() ?? "",
                    Country = ExtractCountryCode(row["Company.Country"]?.ToString()),
                    Last12Months = ParseDouble(row["Last 12 months"]?.ToString()),
                    Balance = ParseDouble(row["Balance"]?.ToString()),
                    FleetSize = ParseInt(row["Company.Fleet Size"]?.ToString()),
                    GrowthRate22_25 = CalculateGrowthRate(value2022, value2025)
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

        private double ParseDouble(string? value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0;
        }

        private int ParseInt(string? value)
        {
            if (int.TryParse(value, out var result))
                return result;
            return 0;
        }
    }
}