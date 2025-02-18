using System;

public static class TimeFormatter
{
    // Compact format: 2h 15m, 1d 4h
    public static string FormatCompact(double duration)
    {
        int sec = (int)(duration % 60);
        int min = (int)((duration / 60) % 60);
        int hr = (int)((duration / 3600) % 24);
        int days = (int)((duration / 86400) % 30);
        int months = (int)((duration / 2592000) % 12);
        int years = (int)(duration / 31536000);

        if (years > 0) return $"{years}y {months}m";
        if (months > 0) return $"{months}m {days}d";
        if (days > 0) return $"{days}d {hr}h";
        if (hr > 0) return $"{hr}h {min}m";
        if (min > 0) return $"{min}m {sec}s";
        return $"{sec}s";
    }

    // Digital Clock format: 02:15:30, 1:02:30:15
    public static string FormatDigital(double duration)
    {
        int sec = (int)(duration % 60);
        int min = (int)((duration / 60) % 60);
        int hr = (int)((duration / 3600) % 24);
        int days = (int)(duration / 86400);

        if (days > 0) return $"{days}:{hr:D2}:{min:D2}:{sec:D2}";
        return $"{hr:D2}:{min:D2}:{sec:D2}";
    }

    // Full Text format: 2 hours 15 minutes, 1 day 4 hours
    public static string FormatFullText(double duration)
    {
        int sec = (int)(duration % 60);
        int min = (int)((duration / 60) % 60);
        int hr = (int)((duration / 3600) % 24);
        int days = (int)((duration / 86400) % 30);
        int months = (int)((duration / 2592000) % 12);
        int years = (int)(duration / 31536000);

        if (years > 0) return $"{years} year{(years > 1 ? "s" : "")} {months} month{(months > 1 ? "s" : "")}";
        if (months > 0) return $"{months} month{(months > 1 ? "s" : "")} {days} day{(days > 1 ? "s" : "")}";
        if (days > 0) return $"{days} day{(days > 1 ? "s" : "")} {hr} hour{(hr > 1 ? "s" : "")}";
        if (hr > 0) return $"{hr} hour{(hr > 1 ? "s" : "")} {min} minute{(min > 1 ? "s" : "")}";
        if (min > 0) return $"{min} minute{(min > 1 ? "s" : "")} {sec} second{(sec > 1 ? "s" : "")}";
        return $"{sec} second{(sec > 1 ? "s" : "")}";
    }

    // Progress Bar format: 1h 45m (50%)
    public static string FormatProgress(double duration, int percentage)
    {
        return $"{FormatCompact(duration)} ({percentage}%)";
    }
}
