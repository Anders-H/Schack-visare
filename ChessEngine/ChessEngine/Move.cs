using System.Drawing;

namespace ChessEngine;

public class Move
{
    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public PieceType Piece { get; }
    public PlayerColor Color { get; }
    public int PieceId { get; }
    public int MoveNumber { get; }

    public Move(Point startPoint, Point endPoint, PieceType piece, PlayerColor color, int pieceId, int moveNumber)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = piece;
        Color = color;
        PieceId = pieceId;
        MoveNumber = moveNumber;
    }   
}
