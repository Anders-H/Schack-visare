#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ChessEngine;

public static class GameFileFormat
{
    private static readonly char[] InvalidFieldCharacters = [';', '\r', '\n'];

    public static string Serialize(string gameName, DateTime gameDate, string whitePlayerName, string blackPlayerName, IReadOnlyList<Move> moves)
    {
        if (moves == null)
            throw new ArgumentNullException(nameof(moves));

        var result = new StringBuilder();
        AppendField(result, gameName, "Game name");
        AppendField(result, gameDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "Game date");
        AppendField(result, whitePlayerName, "White player name");
        AppendField(result, blackPlayerName, "Black player name");

        foreach (var move in moves)
        {
            result.Append(move.ToString());
            result.Append(';');
        }

        return result.ToString();
    }

    private static void AppendField(StringBuilder result, string value, string fieldName)
    {
        if (value == null)
            throw new ArgumentNullException(fieldName);

        if (value.IndexOfAny(InvalidFieldCharacters) >= 0)
        {
            throw new FormatException($@"{fieldName} cannot contain semicolons or line breaks.");
        }

        result.Append(value);
        result.Append(';');
    }
}
