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

        if (piece.Value.PieceId != move.PieceId ||
            piece.Value.Type != move.Piece ||
            piece.Value.Color != move.Color)
        {
            throw new InvalidOperationException(
                $@"The piece at {start} does not match move {move.MoveNumber}.");
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
}
