#nullable enable
using System;
using System.Collections.Generic;
using ChessEngine.Moves;
using ChessEngine.Pieces;

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
    private Move? _lastMove;
    private readonly GamePosition? _initialPosition;
    public Piece? this[int row, int column] => _board[row, column];
    public IReadOnlyList<Piece> DeadPieces => _deadPieces.AsReadOnly();

    private BoardData(BoardData source)
    {
        _board = (Piece?[,])source._board.Clone();
        _deadPieces = new List<Piece>(source._deadPieces);
        _lastMove = source._lastMove;
        _initialPosition = source._initialPosition;
    }

    internal BoardData Copy() => new BoardData(this);

    internal Piece?[,] CopyBoard() => (Piece?[,])_board.Clone();

    public BoardData(GamePosition? initialPosition = null)
    {
        _initialPosition = initialPosition;
        _board = initialPosition?.CopyBoard() ?? new Piece?[8, 8];
        _deadPieces = [];
        if (initialPosition != null)
            return;
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
        if (move.GameEnd != EndingType.MoveIsNotGameEnd)
            return;

        var start = move.StartPoint;
        var end = move.EndPoint;
        var piece = _board[start.Y, start.X];

        if (!piece.HasValue)
            throw new InvalidOperationException($@"No piece at {start}.");

        if (piece.Value.PieceId != move.PieceId || piece.Value.Type != move.Piece || piece.Value.Color != move.Color)
            throw new InvalidOperationException($@"The piece at {start} does not match move {move.MoveNumber}.");

        var promotes = piece.Value.Type == PieceType.Pawn && end.Y == (piece.Value.Color == PlayerColor.White ? 7 : 0);
        if (move.Promotion.HasValue && (!promotes || !Move.IsPromotionPiece(move.Promotion.Value)))
            throw new InvalidOperationException("Invalid promotion for this move.");

        if (piece.Value.Type == PieceType.King &&
            start.Y == end.Y &&
            Math.Abs(start.X - end.X) == 2)
        {
            ApplyCastlingMove(piece.Value, move);
            _lastMove = move;
            return;
        }

        var capturedPiece = _board[end.Y, end.X];
        var enPassant = piece.Value.Type == PieceType.Pawn && start.X != end.X && !capturedPiece.HasValue;

        if (enPassant)
        {
            if (!CanEnPassant(piece.Value, start, end))
                throw new InvalidOperationException("Invalid en passant capture.");

            capturedPiece = _board[start.Y, end.X];
        }

        if (capturedPiece.HasValue)
        {
            var updatedCapturedPiece = capturedPiece.Value;
            updatedCapturedPiece.SetDiedAtMove(move.MoveNumber);
            _deadPieces.Add(updatedCapturedPiece);
        }

        var updatedPiece = piece.Value;
        
        if (promotes)
        {
            updatedPiece = new Piece(updatedPiece.PieceId, move.Promotion ?? PieceType.Queen, updatedPiece.Color)
            {
                MoveCount = updatedPiece.MoveCount,
                DiedAtMove = updatedPiece.DiedAtMove
            };
        }

        updatedPiece.IncreaseMoveCount();
        
        if (enPassant)
            _board[start.Y, end.X] = null;
        
        _board[start.Y, start.X] = null;
        _board[end.Y, end.X] = updatedPiece;
        _lastMove = move;
    }

    public bool CanEnPassant(Piece piece, System.Drawing.Point start, System.Drawing.Point end)
    {
        var direction = piece.Color == PlayerColor.White ? 1 : -1;
        if (piece.Type != PieceType.Pawn || start.Y != (piece.Color == PlayerColor.White ? 4 : 3) ||
            end.Y != start.Y + direction || Math.Abs(end.X - start.X) != 1 ||
            end.X < 0 || end.X > 7 || _board[end.Y, end.X].HasValue)
            return false;
        var captured = _board[start.Y, end.X];
        if (_lastMove == null)
            return _initialPosition != null && _initialPosition.EnPassantTarget == end &&
                _initialPosition.IsWhitesTurn == (piece.Color == PlayerColor.White) &&
                captured is { Type: PieceType.Pawn } && captured.Value.Color != piece.Color;
        return captured is { Type: PieceType.Pawn } && captured.Value.Color != piece.Color &&
            _lastMove.PieceId == captured.Value.PieceId && _lastMove.Piece == PieceType.Pawn &&
            _lastMove.EndPoint == new System.Drawing.Point(end.X, start.Y) &&
            _lastMove.StartPoint == new System.Drawing.Point(end.X, start.Y + 2 * direction);
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
