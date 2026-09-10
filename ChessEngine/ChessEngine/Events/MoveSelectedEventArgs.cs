#nullable enable
using System;
using System.Drawing;
using ChessEngine.Pieces;

namespace ChessEngine.Events;

public sealed class MoveSelectedEventArgs : EventArgs
{
    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public Piece Piece { get; }

    public MoveSelectedEventArgs(Point startPoint, Point endPoint, Piece piece)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Piece = piece;
    }
}
