using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using AbdmWrapperNet.Constants;

namespace AbdmWrapperNet.Common;

public static class Utils
{
    public static string GetCurrentTimeStamp()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    }

    public static bool CheckExpiry(string inputDate)
    {
        if (string.IsNullOrWhiteSpace(inputDate))
            return true;

        if (!inputDate.EndsWith("Z"))
        {
            inputDate += "Z";
        }

        if (DateTime.TryParse(inputDate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var expiryTime))
        {
            return DateTime.UtcNow > expiryTime;
        }

        return true;
    }

    public static string GetSmsExpiry()
    {
        return DateTime.UtcNow.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    }

    public static string SetLinkTokenExpiry()
    {
        return DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    }

    public static DateTime GetCurrentDateTime()
    {
        // Equivalent to the Java implementation offset (UTC+5:30)
        return DateTime.UtcNow.AddHours(5).AddMinutes(30);
    }
}
