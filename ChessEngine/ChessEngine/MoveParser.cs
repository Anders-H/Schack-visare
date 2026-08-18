#nullable enable
using System;
using System.Drawing;

namespace ChessEngine;

public class MoveParser
{
    private readonly string _source;
    private readonly int _moveNumber;

    public MoveParser(string source, int moveNumber)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));

        if (moveNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(moveNumber));

        _moveNumber = moveNumber;
    }

    public MoveParserResult Parse()
    {
        var source = _source.Trim();

        if (source.Length != 5 || source[2] != '-')
        {
            return new MoveParserResult(
                false,
                null,
                @"Expected a move in the form E2-E4.");
        }

        if (!TryParseSquare(source.Substring(0, 2), out var startPoint))
        {
            return new MoveParserResult(
                false,
                null,
                $@"Invalid start square '{source.Substring(0, 2)}'.");
        }

        if (!TryParseSquare(source.Substring(3, 2), out var endPoint))
        {
            return new MoveParserResult(
                false,
                null,
                $@"Invalid end square '{source.Substring(3, 2)}'.");
        }

        var color = _moveNumber % 2 == 0
            ? PlayerColor.White
            : PlayerColor.Black;

        var move = new Move(startPoint, endPoint, color, _moveNumber);
        return new MoveParserResult(true, move, "");
    }

    private static bool TryParseSquare(string source, out Point point)
    {
        var file = char.ToUpperInvariant(source[0]);
        var rank = source[1];

        if (file < 'A' || file > 'H' || rank < '1' || rank > '8')
        {
            point = Point.Empty;
            return false;
        }

        point = new Point(file - 'A', rank - '1');
        return true;
    }
}
