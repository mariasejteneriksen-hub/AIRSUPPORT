using AIRSUPPORT.Components.Models;

namespace AIRSUPPORT.Components.Services
{
    public class CustomerRevenueService
    {
        private readonly MasterDataService _masterDataService = new();

        public List<(string CustomerNr, string CompanyName)> GetCustomerList()
        {
            var customerNrsWithLines = _masterDataService.LoadLines()
                .Select(l => l.CustomerNr)
                .ToHashSet();

            return _masterDataService.LoadCustomers()
                .Where(c => !string.IsNullOrWhiteSpace(c.CompanyName)
                         && !string.IsNullOrWhiteSpace(c.CustomerNr)
                         && customerNrsWithLines.Contains(c.CustomerNr))
                .Select(c => (CustomerNr: c.CustomerNr!, CompanyName: c.CompanyName!))
                .OrderBy(c => c.CompanyName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<(string Program, List<(string MonthLabel, double Revenue, List<(string Description, double Amount)> Breakdown)> Points)> GetMonthlyRevenueByProgram(string customerNr)
        {
            var lines = _masterDataService.LoadLines()
                .Where(l => l.CustomerNr == customerNr && l.PostingDate.HasValue);

            return lines
                .GroupBy(l => new
                {
                    Program = string.IsNullOrWhiteSpace(l.PostingGroupUpdated) ? "Andet" : l.PostingGroupUpdated!,
                    l.PostingDate!.Value.Year,
                    l.PostingDate.Value.Month
                })
                .Select(g => new
                {
                    g.Key.Program,
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(l => l.AmountLCY),
                    // Gruppér på kombinationen af Description og Description2
                    Breakdown = g.GroupBy(l =>
                    {
                        var mainDesc = string.IsNullOrWhiteSpace(l.Description) ? "Uden beskrivelse" : l.Description.Trim();
                        var subDesc = l.Description2; // Tilpas feltnavnet hvis det hedder noget andet i din model (f.eks. l.Description2)

                        // Hvis Description2 er udfyldt, tilføjes den i parentes eller efter bindestreg
                        return !string.IsNullOrWhiteSpace(subDesc)
                    ? $"{mainDesc} ({subDesc.Trim()})"
                    : mainDesc;
                    })
                        .Select(bg => (Description: bg.Key, Amount: bg.Sum(l => l.AmountLCY)))
                        .OrderByDescending(b => b.Amount)
                        .ToList()
                })
                .GroupBy(g => g.Program)
                .Select(programGroup => (
                    Program: programGroup.Key,
                    Points: programGroup
                        .OrderBy(g => g.Year).ThenBy(g => g.Month)
                        .Select(g => ($"{g.Year}-{g.Month:00}", g.Revenue, g.Breakdown))
                        .ToList()
                ))
                .ToList();
        }
    }
}