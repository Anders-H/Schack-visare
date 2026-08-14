#nullable enable
namespace ChessEngine;

public struct Piece
{
    public PieceType Type { get; }
    public PlayerColor Color { get; }
    public int PieceId { get; }

    public Piece(int pieceId, PieceType type, PlayerColor color)
    {
        PieceId = pieceId;
        Type = type;
        Color = color;
    }
}