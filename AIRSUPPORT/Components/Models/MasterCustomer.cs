namespace AIRSUPPORT.Components.Models
{
    public class MasterCustomer
    {
        public string? CustomerNr { get; set; }
        public string? NavCustomerNo { get; set; }
        public string? CompanyName { get; set; }
        public string? CustomerStatus { get; set; }
        public double PriorityCustomer { get; set; }
        public double TailsOnPPS { get; set; }
        public double TailsFlightWatchTerrestrial { get; set; }
        public double TailsFlightWatchSatellite { get; set; }
        public double TailsNotamMonitoring { get; set; }
        public DateTime? CustomerSince { get; set; }
        public decimal? PriceEscalation { get; set; }
        public double? ChurnValue { get; set; }
        public DateTime? ChurnDate { get; set; }          // NYT — erstatter Terminationdate/ChurnReportedDate/BlockedDate
        public bool? Avoidable { get; set; }               // NYT
        public string? CombinedReason { get; set; }         // NYT — erstatter ChurnReason/BlockedReason
        public string? Region { get; set; }                 // NYT
        public int? Tier { get; set; }                       // NYT
        public string? Country { get; set; }
        public double Balance { get; set; }
        public int FleetSize { get; set; }
        public double Last12Months { get; set; }
        public double YearToDate { get; set; }
        public double LastYearToDate { get; set; }
        public double LastFinancialYear { get; set; }
        public string? NavCustomerName { get; set; }
        public string? NavCountry { get; set; }
        public int? NavBlocked { get; set; }
        public double Turnover2017 { get; set; }
        public double Turnover2018 { get; set; }
        public double Turnover2019 { get; set; }
        public double Turnover2020 { get; set; }
        public double Turnover2021 { get; set; }
        public double Turnover2022 { get; set; }
        public double Turnover2023 { get; set; }
        public double Turnover2024 { get; set; }
        public double Turnover2025 { get; set; }
        public double Turnover2026 { get; set; }
        public int? Dev1718 { get; set; }
        public int? Dev1819 { get; set; }
        public int? Dev1920 { get; set; }
        public int? Dev2021 { get; set; }
        public int? Dev2122 { get; set; }
        public int? Dev2223 { get; set; }
        public int? Dev2324 { get; set; }
        public int? Dev2425 { get; set; }
        public int? Dev2526 { get; set; }

        // RFM-features, udledt af fakturalinjerne (master_lines) i SQL og gemt som rigtige
        // kolonner på master_customers — se dataordbogen for definitionerne.
        public int? DaysSinceLastInvoice { get; set; }
        public int? InvoiceCount { get; set; }
        public double? AvgDaysBetweenInvoices { get; set; }
        public int? InvoiceCountLast6M { get; set; }
        public int? InvoiceCountPrev6M { get; set; }
        public int? PPSInvoiceCount { get; set; }
        public int? OCInvoiceCount { get; set; }
        public int? ActiveProgramCount { get; set; }
        public double? AvgLineAmount { get; set; }

        // Udledte egenskaber (beregnes ikke fra CSV, men ud fra felterne ovenfor) —
        // samlet ét sted, så Statistik/Forretningsanalyse/Korrelation bruger nøjagtig samme formel.

        // Antal år som kunde. Bruger ChurnDate som slutdato for churnede kunder i stedet for i dag —
        // ellers ville tenure for en kunde, der churnede i fx 2022, fortsætte med at vokse frem til i dag.
        public double? LifetimeYears =>
            CustomerSince.HasValue
                ? ((ChurnDate ?? DateTime.Now) - CustomerSince.Value).TotalDays / 365.25
                : null;

        // Gennemsnitlig årlig omsætning. Bruger kun de 9 fulde år (2017-2025) — Turnover2026 er et
        // ufuldstændigt år og holdes udenfor. Nævneren er MIN(LifetimeYears, 9): nye kunder deles med
        // deres faktiske levetid, mens kunder fra før 2017 cappes ved 9, da det er den periode, vi har data for.
        public double? AverageTurnover
        {
            get
            {
                if (LifetimeYears is not double lifetime || lifetime <= 0)
                    return null;

                var sum = Turnover2017 + Turnover2018 + Turnover2019 + Turnover2020 + Turnover2021
                        + Turnover2022 + Turnover2023 + Turnover2024 + Turnover2025;

                return sum / Math.Min(lifetime, 9.0);
            }
        }

        // Faktureringstrend: positiv = flere fakturaer for nylig end tidligere, negativ = færre (advarselstegn).
        // Erstatter InvoiceCountLast6M/InvoiceCountPrev6M som model-input — de to rå tal korrelerer stærkt
        // (r=0,90), fordi de fleste kunder er stabile, men differencen fanger netop de kunder, der ikke er.
        public int? InvoiceTrend =>
            InvoiceCountLast6M.HasValue && InvoiceCountPrev6M.HasValue
                ? InvoiceCountLast6M - InvoiceCountPrev6M
                : null;
    }
}