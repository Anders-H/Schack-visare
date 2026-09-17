#nullable enable
using System;
using System.Drawing;
using ChessEngine.Pieces;

namespace ChessEngine.Moves;

public class MoveParser
{
    private readonly string _source;
    private readonly int _moveNumber;
    private readonly bool _startsWithWhite;

    public MoveParser(string source, int moveNumber, bool startsWithWhite = true)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));

        if (moveNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(moveNumber));

        _moveNumber = moveNumber;
        _startsWithWhite = startsWithWhite;
    }

    public MoveParserResult Parse()
    {
        var source = _source.Trim();

        if ((source.Length != 5 && source.Length != 7) || source[2] != '-')
        {
            return new MoveParserResult(
                false,
                null,
                @"Expected a move in the form E2-E4 or E7-E8=Q.");
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

        var color = (_moveNumber % 2 == 0) == _startsWithWhite
            ? PlayerColor.White
            : PlayerColor.Black;

        PieceType? promotion = null;
        if (source.Length == 7)
        {
            if (source[5] != '=' || endPoint.Y != (color == PlayerColor.White ? 7 : 0))
                return new MoveParserResult(false, null, "Invalid promotion suffix or destination.");
            try { promotion = Move.ParsePromotion(source[6]); }
            catch (FormatException ex) { return new MoveParserResult(false, null, ex.Message); }
        }
        var move = new Move(startPoint, endPoint, color, _moveNumber, promotion);
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
