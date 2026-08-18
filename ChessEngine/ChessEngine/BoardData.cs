#nullable enable
using System;
using System.Collections.Generic;

namespace ChessEngine;

public class BoardData
{
    private static readonly PieceType[] BackRank =
    [
        PieceType.Rook,
        PieceType.Knight,
        PieceType.Bishop,
        PieceType.Queen,
        PieceType.King,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook
    ];

    private readonly Piece?[,] _board;
    private readonly List<Piece> _deadPieces;
    public Piece? this[int row, int column] => _board[row, column];

    public BoardData()
    {
        _board = new Piece?[8, 8];
        _deadPieces = [];
        InitializeBackRank(0, PlayerColor.White, 0);
        InitializePawnRank(1, PlayerColor.White, 8);
        InitializeBackRank(7, PlayerColor.Black, 16);
        InitializePawnRank(6, PlayerColor.Black, 24);
    }

    private void InitializeBackRank(int row, PlayerColor color, int firstPieceId)
    {
        for (var column = 0; column < BackRank.Length; column++)
        {
            _board[row, column] = new Piece(
                firstPieceId + column,
                BackRank[column],
                color);
        }
    }

    private void InitializePawnRank(int row, PlayerColor color, int firstPieceId)
    {
        for (var column = 0; column < 8; column++)
        {
            _board[row, column] = new Piece(
                firstPieceId + column,
                PieceType.Pawn,
                color);
        }
    }

    public void ApplyMove(Move move)
    {
        var start = move.StartPoint;
        var end = move.EndPoint;
        var piece = _board[start.Y, start.X];

        if (!piece.HasValue)
            throw new InvalidOperationException($@"No piece at {start}.");

        if (piece.Value.PieceId != move.PieceId || piece.Value.Type != move.Piece || piece.Value.Color != move.Color)
            throw new InvalidOperationException($@"The piece at {start} does not match move {move.MoveNumber}.");

        if (piece.Value.Type == PieceType.King &&
            start.Y == end.Y &&
            Math.Abs(start.X - end.X) == 2)
        {
            ApplyCastlingMove(piece.Value, move);
            return;
        }

        var capturedPiece = _board[end.Y, end.X];

        if (capturedPiece.HasValue)
        {
            var updatedCapturedPiece = capturedPiece.Value;
            updatedCapturedPiece.SetDiedAtMove(move.MoveNumber);
            _deadPieces.Add(updatedCapturedPiece);
        }

        var updatedPiece = piece.Value;
        updatedPiece.IncreaseMoveCount();
        _board[start.Y, start.X] = null;
        _board[end.Y, end.X] = updatedPiece;
    }

    private void ApplyCastlingMove(Piece king, Move move)
    {
        var start = move.StartPoint;
        var end = move.EndPoint;
        var kingSide = end.X > start.X;
        var rookStartColumn = kingSide ? 7 : 0;
        var rookEndColumn = kingSide ? 5 : 3;
        var rook = _board[start.Y, rookStartColumn];

        if (!rook.HasValue ||
            rook.Value.Type != PieceType.Rook ||
            rook.Value.Color != king.Color)
        {
            throw new InvalidOperationException(
                @"Castling cannot be applied because the expected rook is missing.");
        }

        if (_board[end.Y, end.X].HasValue ||
            _board[start.Y, rookEndColumn].HasValue)
        {
            throw new InvalidOperationException(
                @"Castling cannot be applied because a destination square is occupied.");
        }

        var updatedKing = king;
        updatedKing.IncreaseMoveCount();

        var updatedRook = rook.Value;
        updatedRook.IncreaseMoveCount();

        _board[start.Y, start.X] = null;
        _board[end.Y, end.X] = updatedKing;
        _board[start.Y, rookStartColumn] = null;
        _board[start.Y, rookEndColumn] = updatedRook;
    }
}
