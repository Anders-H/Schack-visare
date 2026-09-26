#nullable enable
using System.Drawing;
using ChessEngine.Pieces;

namespace ChessEngine;

public sealed class GamePosition
{
    private readonly Piece?[,] _board;
    public string Fen { get; }
    public bool IsWhitesTurn { get; }
    public Point? EnPassantTarget { get; }
    public int HalfmoveClock { get; }
    public int FullmoveNumber { get; }

    internal GamePosition(Piece?[,] board, string fen, bool isWhitesTurn, Point? enPassantTarget, int halfmoveClock, int fullmoveNumber)
    {
        _board = (Piece?[,])board.Clone();
        Fen = fen;
        IsWhitesTurn = isWhitesTurn;
        EnPassantTarget = enPassantTarget;
        HalfmoveClock = halfmoveClock;
        FullmoveNumber = fullmoveNumber;
    }

    internal Piece?[,] CopyBoard() => (Piece?[,])_board.Clone();
}
