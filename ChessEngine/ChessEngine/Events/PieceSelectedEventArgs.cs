using System;
using System.Drawing;
using ChessEngine.Pieces;

namespace ChessEngine.Events;

public sealed class PieceSelectedEventArgs : EventArgs
{
    public Point Point { get; }
    public Piece? Piece { get; }

    public PieceSelectedEventArgs(Point point, Piece? piece)
    {
        Point = point;
        Piece = piece;
    }
}
