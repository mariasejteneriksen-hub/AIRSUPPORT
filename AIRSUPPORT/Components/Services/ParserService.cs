using System.Globalization;

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
    }
}