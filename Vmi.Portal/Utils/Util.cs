using System.Globalization;

namespace Vmi.Portal.Utils
{
    public class Util
    {
        public static DateTime PegaHoraBrasilia()
        {
            try
            {
                return TimeZoneInfo.ConvertTime(
                    DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static Task<DateTime> PegaHoraBrasiliaAsync()
        {
            return Task.FromResult(PegaHoraBrasilia());
        }

        public static bool ConverterDataParaDDMMYYYY(string dateStr, string format, out DateTime? result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(dateStr))
                return true;

            if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                result = parsedDate;
                return true;
            }

            return false;
        }
    }
}
