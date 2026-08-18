using System.Drawing;

namespace ChessEngine;

public class Move
{
    private const int BoardLength = 8;

    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public PieceType? Piece { get; }
    public PlayerColor Color { get; }
    public int? PieceId { get; }
    public int MoveNumber { get; }

    public Move(Point startPoint, Point endPoint, PlayerColor color, int moveNumber)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = null;
        Color = color;
        PieceId = null;
        MoveNumber = moveNumber;
    }

    public Move(Point startPoint, Point endPoint, PieceType piece, PlayerColor color, int pieceId, int moveNumber)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = piece;
        Color = color;
        PieceId = pieceId;
        MoveNumber = moveNumber;
    }

    public static string FormatSquare(Point square)
    {
        if (square.X < 0 || square.X >= BoardLength ||
            square.Y < 0 || square.Y >= BoardLength)
        {
            throw new System.FormatException($@"Invalid board coordinate: {square}.");
        }

        var file = (char)('A' + square.X);
        var rank = square.Y + 1;
        return $@"{file}{rank}";
    }

    public override string ToString() =>
        $@"{FormatSquare(StartPoint)}-{FormatSquare(EndPoint)}";
}
