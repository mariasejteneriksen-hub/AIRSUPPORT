using AIRSUPPORT.Components.Models;
using CsvHelper;
using System.Globalization;

namespace AIRSUPPORT.Components.Services
{
    public class MasterDataService
    {
        public List<MasterCustomer> LoadCustomers()
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                BadDataFound = null
            };

            using var reader = new StreamReader("wwwroot/Data/CustomerCSV1.csv");
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            var rawRows = new List<dynamic>();
            while (csv.Read())
            {
                rawRows.Add(csv.GetRecord<dynamic>());
            }

            return rawRows.Select(r =>
            {
                var row = (IDictionary<string, object>)r;

                return new MasterCustomer
                {
                    CustomerNr = row["CustomerNr"]?.ToString(),
                    NavCustomerNo = row["NavCustomerNo"]?.ToString(),
                    CompanyName = row["CompanyName"]?.ToString(),
                    CustomerStatus = row["CustomerStatus"]?.ToString(),
                    PriorityCustomer = CsvParsingHelper.ParseDoubleDanish(row["PriorityCustomer"]?.ToString()),
                    TailsOnPPS = CsvParsingHelper.ParseDoubleDanish(row["TailsOnPPS"]?.ToString()),
                    TailsFlightWatchTerrestrial = CsvParsingHelper.ParseDoubleDanish(row["TailsFlightWatchTerrestrial"]?.ToString()),
                    TailsFlightWatchSatellite = CsvParsingHelper.ParseDoubleDanish(row["TailsFlightWatchSatellite"]?.ToString()),
                    TailsNotamMonitoring = CsvParsingHelper.ParseDoubleDanish(row["TailsNotamMonitoring"]?.ToString()),
                    CustomerSince = CsvParsingHelper.ParseDate(row["CustomerSince"]?.ToString()),
                    PriceEscalation = CsvParsingHelper.ParseDecimal(row["PriceEscalation"]?.ToString()),
                    Terminationdate = CsvParsingHelper.ParseDate(row["Terminationdate"]?.ToString()),
                    ChurnValue = CsvParsingHelper.ParseDoubleDanish(row["ChurnValue"]?.ToString()),
                    ChurnReportedDate = CsvParsingHelper.ParseDate(row["ChurnReportedDate"]?.ToString()),
                    ChurnDate = CsvParsingHelper.ParseDate(row["ChurnDate"]?.ToString()),                    // NYT
                    Avoidable = CsvParsingHelper.ParseAvoidable(row["Avoidable"]?.ToString()),                 // NYT
                    ChurnReason = CsvParsingHelper.CleanNullText(row["ChurnReason"]?.ToString()),
                    BlockedDate = CsvParsingHelper.ParseDate(row["BlockedDate"]?.ToString()),
                    CombinedReason = CsvParsingHelper.CleanNullText(row["CombinedReason"]?.ToString()),        // NYT
                    BlockedReason = CsvParsingHelper.CleanNullText(row["BlockedReason"]?.ToString()),
                    Region = CsvParsingHelper.CleanNullText(row["Region"]?.ToString()),                        // NYT
                    Tier = CsvParsingHelper.ParseNullableInt(row["Tier"]?.ToString()),                          // NYT
                    Country = CsvParsingHelper.CleanNullText(row["Country"]?.ToString()),
                    Balance = CsvParsingHelper.ParseDouble(row["Balance"]?.ToString()),
                    FleetSize = CsvParsingHelper.ParseInt(row["FleetSize"]?.ToString()),
                    Last12Months = CsvParsingHelper.ParseDouble(row["Last12Months"]?.ToString()),
                    YearToDate = CsvParsingHelper.ParseDouble(row["YearToDate"]?.ToString()),
                    LastYearToDate = CsvParsingHelper.ParseDouble(row["LastYearToDate"]?.ToString()),
                    LastFinancialYear = CsvParsingHelper.ParseDouble(row["LastFinancialYear"]?.ToString()),
                    NavCustomerName = row["NavCustomerName"]?.ToString(),
                    NavCountry = row["NavCountry"]?.ToString(),
                    NavBlocked = CsvParsingHelper.ParseNullableInt(row["NavBlocked"]?.ToString()),
                    Turnover2017 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2017"]?.ToString()),
                    Turnover2018 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2018"]?.ToString()),
                    Turnover2019 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2019"]?.ToString()),
                    Turnover2020 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2020"]?.ToString()),
                    Turnover2021 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2021"]?.ToString()),
                    Turnover2022 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2022"]?.ToString()),
                    Turnover2023 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2023"]?.ToString()),
                    Turnover2024 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2024"]?.ToString()),
                    Turnover2025 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2025"]?.ToString()),
                    Turnover2026 = CsvParsingHelper.ParseDoubleDanish(row["Turnover2026"]?.ToString()),
                    Dev1718 = CsvParsingHelper.ParseNullableInt(row["Dev1718"]?.ToString()),
                    Dev1819 = CsvParsingHelper.ParseNullableInt(row["Dev1819"]?.ToString()),
                    Dev1920 = CsvParsingHelper.ParseNullableInt(row["Dev1920"]?.ToString()),
                    Dev2021 = CsvParsingHelper.ParseNullableInt(row["Dev2021"]?.ToString()),
                    Dev2122 = CsvParsingHelper.ParseNullableInt(row["Dev2122"]?.ToString()),
                    Dev2223 = CsvParsingHelper.ParseNullableInt(row["Dev2223"]?.ToString()),
                    Dev2324 = CsvParsingHelper.ParseNullableInt(row["Dev2324"]?.ToString()),
                    Dev2425 = CsvParsingHelper.ParseNullableInt(row["Dev2425"]?.ToString()),
                    Dev2526 = CsvParsingHelper.ParseNullableInt(row["Dev2526"]?.ToString()),
                };
            }).ToList();
        }

        public List<MasterLine> LoadLines()
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                BadDataFound = null
            };

            using var reader = new StreamReader("wwwroot/Data/FakturaCSV1.csv");
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            var rawRows = new List<dynamic>();
            while (csv.Read())
            {
                rawRows.Add(csv.GetRecord<dynamic>());
            }

            return rawRows.Select(r =>
            {
                var row = (IDictionary<string, object>)r;

                return new MasterLine
                {
                    CustomerNr = row["CustomerNr"]?.ToString(),
                    SellToCustomerNo = row["Sell-to Customer No_"]?.ToString(),
                    BillToCustomerNo = row["Bill-to Customer No_"]?.ToString(),
                    DocumentNo = row["Document No_"]?.ToString(),
                    LineNo = CsvParsingHelper.ParseInt(row["Line No_"]?.ToString()),
                    Type = CsvParsingHelper.ParseInt(row["Type"]?.ToString()),
                    ItemNr = row["ItemNr"]?.ToString(),
                    Description = row["Description"]?.ToString(),
                    Description2 = row["Description 2"]?.ToString(),
                    Quantity = CsvParsingHelper.ParseDoubleDanish(row["Quantity"]?.ToString()),
                    UnitPrice = CsvParsingHelper.ParseDoubleDanish(row["UnitPrice"]?.ToString()),
                    UnitPriceLCY = CsvParsingHelper.ParseDoubleDanish(row["UnitPriceLCY"]?.ToString()),
                    Amount = CsvParsingHelper.ParseDoubleDanish(row["Amount"]?.ToString()),
                    Currency = row["Currency"]?.ToString(),
                    CurrencyFactor = CsvParsingHelper.ParseDoubleDanish(row["CurrencyFactor"]?.ToString()),
                    AmountLCY = CsvParsingHelper.ParseDoubleDanish(row["AmountLCY"]?.ToString()),
                    AmountInclVAT = CsvParsingHelper.ParseDoubleDanish(row["AmountInclVAT"]?.ToString()),
                    VATPercent = CsvParsingHelper.ParseDoubleDanish(row["VAT%"]?.ToString()),
                    PostingGroupUpdated = CsvParsingHelper.CleanNullText(row["PostingGroupUpdated"]?.ToString()),
                    GenProdPostingGroup = row["Gen_ Prod_ Posting Group"]?.ToString(),
                    VATBusPostingGroup = row["VAT Bus_ Posting Group"]?.ToString(),
                    SalesType = row["SalesType"]?.ToString(),
                    PostingDate = CsvParsingHelper.ParseDate(row["Posting Date"]?.ToString()),
                    NavCustomerNo = row["NavCustomerNo"]?.ToString(),
                };
            }).ToList();
        }
    }
}
