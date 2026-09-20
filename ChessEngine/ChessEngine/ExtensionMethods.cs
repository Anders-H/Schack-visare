#nullable enable
using System;

namespace ChessEngine;

public static class ExtensionMethods
{
    public static bool ParseDate(this string? me, out DateTime date)
    {
        date = DateTime.Now;
        me = (me ?? "").Trim();

        var parts = me.Split('-');

        try
        {
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month) || !int.TryParse(parts[2], out var day))
                return false;

            if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31)
                return false;

            date = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
