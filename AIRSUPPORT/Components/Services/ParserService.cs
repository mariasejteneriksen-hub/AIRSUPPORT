using System.Globalization;
using System.Text.RegularExpressions;

namespace AIRSUPPORT.Components.Services
{
    public static class CsvParsingHelper
    {
        public static double ParseDouble(string? value)
        {
            //if (string.IsNullOrWhiteSpace(value))
            //    return 0;

            //var cleaned = value
            //    .Replace(" ", "")
            //    .Replace("\u00A0", "")
            //    .Replace("%", "");

            //if (double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            //    return result;

            //return 0;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0;
        }

        public static int ParseInt(string? value)
        {
            if (int.TryParse(value, out var result))
                return result;
            return 0;
        }

        public static DateTime? ParseDate(string? value)
        {
            if (DateTime.TryParse(value, out var result))
                return result;
            return null;
        }

        public static decimal? ParseDecimal(string? value)
        {
            if (decimal.TryParse(value, out var result))
                return result;
            return null;
        }

        public static (string? Region, int? Tier) ParseRegionTier(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (null, null);

            var match = Regex.Match(value.Trim(), @"^([A-Za-z]+)(\d+)$");
            if (!match.Success)
                return (null, null);

            return (match.Groups[1].Value, int.Parse(match.Groups[2].Value));
        }

        public static double ParseDoubleDanish(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            var cleaned = value
                .Replace(".", "")   // fjern tusind-separator (punktum)
                .Replace(",", "."); // gør decimal-komma til punktum

            if (double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0;
        }
        public static DateTime? GetEarliestDate(params DateTime?[] dates)
        {
            var validDates = dates.Where(d => d.HasValue).Select(d => d!.Value);
            return validDates.Any() ? validDates.Min() : null;
        }

        public static string? CombineReasons(string? reason1, string? reason2)
        {
            var hasReason1 = !string.IsNullOrWhiteSpace(reason1);
            var hasReason2 = !string.IsNullOrWhiteSpace(reason2);

            if (hasReason1 && hasReason2)
                return $"{reason1} | {reason2}"; // begge udfyldt: kombinér med en tydelig adskiller

            if (hasReason1)
                return reason1; // kun den ene

            if (hasReason2)
                return reason2; // kun den anden

            return null; // ingen af dem
        }

        public static bool? ParseAvoidable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null; // ingenting angivet

            var normalized = value.Trim().ToLower();

            if (normalized == "avoidable")
                return true;

            if (normalized == "unavoidable")
                return false;

            return null; // uventet værdi, sikrere at returnere null end at gætte forkert
        }
    }
}