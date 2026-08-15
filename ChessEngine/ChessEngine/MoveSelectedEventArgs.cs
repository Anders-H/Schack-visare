#nullable enable
using System;
using System.Drawing;

namespace ChessEngine;

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
