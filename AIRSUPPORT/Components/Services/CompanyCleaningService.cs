using AIRSUPPORT.Components.Models;
using CsvHelper;
using System.Globalization;
using System.Timers;

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
                var (region, tier) = CsvParsingHelper.ParseRegionTier(row["Region + Tier"]?.ToString());

                // NYT: gem de tre datoer i variable FØRST, så vi kan bruge dem to gange
                var terminationAsPer = CsvParsingHelper.ParseDate(row["Termination as per"]?.ToString());
                var blockedDate = CsvParsingHelper.ParseDate(row["Blocked Date"]?.ToString());
                var churnReportedDate = CsvParsingHelper.ParseDate(row["Churn Reported Date"]?.ToString());
                var churnReason = row["Churn reason"]?.ToString();
                var blockedReason = row["Blocked Reason"]?.ToString();

                return new CleanedCompany
                {
                    NavCustomerNo = row["Nav Customer No"]?.ToString(),
                    CompanyName = row["Company name"]?.ToString(),
                    CustomerStatus = row["Customer Status"]?.ToString(),
                    PriorityCustomer = CsvParsingHelper.ParseDouble(row["Priority Customer"]?.ToString()),
                    TailsOnPPS = CsvParsingHelper.ParseDouble(row["Tails – PPS"]?.ToString()?.Replace(",", ".")),
                    TailsFlightWatchTerrestrial = CsvParsingHelper.ParseDouble(row["Tails – OC-FW, Terr"]?.ToString()?.Replace(",", ".")),
                    TailsFlightWatchSatellite = CsvParsingHelper.ParseDouble(row["Tails – OC-FW, SAT"]?.ToString()?.Replace(",", ".")),
                    TailsNotamMonitoring = CsvParsingHelper.ParseDouble(row["Tails - OC-NOTAM"]?.ToString()?.Replace(",", ".")),
                    CustomerSince = CsvParsingHelper.ParseDate(row["Customer Since"]?.ToString()),
                    PriceEscalation = CsvParsingHelper.ParseDecimal(row["Price escalation %"]?.ToString()?.Replace(",", ".")),
                    Terminationdate = terminationAsPer,          // ÆNDRET: brug variablen i stedet for at parse igen
                    ChurnValue = CsvParsingHelper.ParseDoubleDanish(row["Churn value"]?.ToString()),
                    ChurnReportedDate = churnReportedDate,        // ÆNDRET: brug variablen i stedet for at parse igen
                    Avoidable = CsvParsingHelper.ParseAvoidable(row["Avoidable/Un-avoidable"]?.ToString()),
                    ChurnReason = churnReason,
                    BlockedDate = blockedDate,                    // ÆNDRET: brug variablen i stedet for at parse igen
                    BlockedReason = blockedReason,
                    CombinedReason = CsvParsingHelper.CombineReasons(churnReason, blockedReason),
                    Region = region,
                    Tier = tier,
                    ChurnDate = CsvParsingHelper.GetEarliestDate(terminationAsPer, blockedDate, churnReportedDate), // NYT
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