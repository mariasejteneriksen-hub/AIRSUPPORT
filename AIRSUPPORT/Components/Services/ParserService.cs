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
    }
}