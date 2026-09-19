using System.Drawing;
using ChessEngine.Pieces;

namespace ChessEngine.Moves;

public class Move
{
    private const int BoardLength = 8;
    public EndingType GameEnd { get; }
    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public PieceType? Piece { get; }
    public PlayerColor Color { get; }
    public int? PieceId { get; }
    public int MoveNumber { get; }
    public PieceType? Promotion { get; }

    public Move(Point startPoint, Point endPoint, PlayerColor color, int moveNumber, PieceType? promotion = null)
    {
        GameEnd = EndingType.MoveIsNotGameEnd;
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = null;
        Color = color;
        PieceId = null;
        MoveNumber = moveNumber;
        Promotion = promotion;
    }

    public Move(Point startPoint, Point endPoint, PieceType piece, PlayerColor color, int pieceId, int moveNumber, PieceType? promotion = null)
    {
        GameEnd = EndingType.MoveIsNotGameEnd;
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = piece;
        Color = color;
        PieceId = pieceId;
        MoveNumber = moveNumber;
        Promotion = promotion;
    }

    public Move(Point startPoint, Point endPoint, Piece piece, int moveNumber, PieceType? promotion = null)
    {
        GameEnd = EndingType.MoveIsNotGameEnd;
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = piece.Type;
        Color = piece.Color;
        PieceId = piece.PieceId;
        MoveNumber = moveNumber;
        Promotion = promotion;
    }

    public Move(RegisterEndingType endingType, int moveNumber)
    {
        switch (endingType)
        {
            case RegisterEndingType.WhiteWins:
                GameEnd = EndingType.WhiteWins;
                Color = PlayerColor.White;
                break;
            case RegisterEndingType.BlackWins:
                GameEnd = EndingType.BlackWins;
                Color = PlayerColor.Black;
                break;
            case RegisterEndingType.Draw:
                GameEnd = EndingType.Draw;
                Color = PlayerColor.White;
                break;
        }

        StartPoint = new Point(-1, -1);
        EndPoint = new Point(-1, -1);
        Piece = null;
        PieceId = null;
        MoveNumber = moveNumber;
        Promotion = null;
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

    public override string ToString() => GameEnd switch
    {
        EndingType.WhiteWins => "END=WHITE",
        EndingType.BlackWins => "END=BLACK",
        EndingType.Draw => "END=DRAW",
        _ => $@"{FormatSquare(StartPoint)}-{FormatSquare(EndPoint)}" + (Promotion.HasValue ? "=" + PromotionSymbol(Promotion.Value) : "")
    };

    public static bool IsPromotionPiece(PieceType type) =>
        type is PieceType.Queen or PieceType.Rook or PieceType.Bishop or PieceType.Knight;

    public static string PromotionSymbol(PieceType type) => type switch
    {
        PieceType.Queen => "Q", PieceType.Rook => "R", PieceType.Bishop => "B", PieceType.Knight => "N",
        _ => throw new System.FormatException("Promotion must be to Q, R, B or N.")
    };

    public static PieceType ParsePromotion(char symbol) => char.ToUpperInvariant(symbol) switch
    {
        'Q' => PieceType.Queen, 'R' => PieceType.Rook, 'B' => PieceType.Bishop, 'N' => PieceType.Knight,
        _ => throw new System.FormatException("Promotion must be to Q, R, B or N.")
    };
}
