#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace ChessEngine;

public static class GameFileFormat
{
    private const int BoardLength = 8;
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
            result.Append(FormatSquare(move.StartPoint));
            result.Append('-');
            result.Append(FormatSquare(move.EndPoint));
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

    private static string FormatSquare(Point square)
    {
        if (square.X < 0 || square.X >= BoardLength ||
            square.Y < 0 || square.Y >= BoardLength)
        {
            throw new FormatException($@"Invalid board coordinate: {square}.");
        }

        var file = (char)('A' + square.X);
        var rank = square.Y + 1;
        return $@"{file}{rank}";
    }
}
