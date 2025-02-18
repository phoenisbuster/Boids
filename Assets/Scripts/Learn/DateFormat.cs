using System;
using System.Text.RegularExpressions;

/**
 * Formats a DateTime object based on a given format string.
 *
 * - yyyy → Full year (2025), yy → Short year (25)
 * - mm → Two-digit month (02), m → Single-digit month (2)
 * - dd → Two-digit day (09), d → Single-digit day (9)
 * - HH → Two-digit 24-hour (09), H → Single-digit 24-hour (9)
 * - hh → Two-digit 12-hour (09), h → Single-digit 12-hour (9)
 * - MM → Two-digit minute (05), M → Single-digit minute (5)
 * - ss → Two-digit second (08), s → Single-digit second (8)
 * - A → AM/PM, a → am/pm
 */
public static class DateFormatter
{
    public static string FormatDate(DateTime date, string format)
    {
        int hours12 = date.Hour % 12 == 0 ? 12 : date.Hour % 12; // Convert 24-hour to 12-hour format

        return Regex.Replace(format, @"yyyy|yy|mm|m|dd|d|HH|H|hh|h|MM|M|ss|s|A|a", match =>
        {
            return match.Value switch
            {
                "yyyy" => date.Year.ToString(),
                "yy" => date.Year.ToString().Substring(2),
                "mm" => date.Month.ToString("D2"),
                "m" => date.Month.ToString(),
                "dd" => date.Day.ToString("D2"),
                "d" => date.Day.ToString(),
                "HH" => date.Hour.ToString("D2"),
                "H" => date.Hour.ToString(),
                "hh" => hours12.ToString("D2"),
                "h" => hours12.ToString(),
                "MM" => date.Minute.ToString("D2"),
                "M" => date.Minute.ToString(),
                "ss" => date.Second.ToString("D2"),
                "s" => date.Second.ToString(),
                "A" => date.Hour >= 12 ? "PM" : "AM",
                "a" => date.Hour >= 12 ? "pm" : "am",
                _ => match.Value,
            };
        });
    }
}