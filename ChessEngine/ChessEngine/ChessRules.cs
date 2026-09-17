#nullable enable
using System;
using System.Drawing;
using ChessEngine.Moves;
using ChessEngine.Pieces;

namespace ChessEngine;

public class ChessRules
{
    private readonly bool _isWhitesTurn;
    private readonly MoveList _moves;

    public ChessRules(bool isWhitesTurn, MoveList moves)
    {
        _isWhitesTurn = isWhitesTurn;
        _moves = moves ?? throw new ArgumentNullException(nameof(moves));
    }

    public bool IsMoveLegal(Piece piece, Point startPoint, Point endPoint, PieceType? promotion = null)
    {
        if (!Inside(startPoint) || !Inside(endPoint) || startPoint == endPoint || piece.Color != (_isWhitesTurn ? PlayerColor.White : PlayerColor.Black))
            return false;

        var position = new BoardData(_moves.InitialPosition);

        foreach (var move in _moves)
        {
            if (!Inside(move.StartPoint) || !Inside(move.EndPoint))
                return false;

            try
            {
                position.ApplyMove(move);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        var board = new Piece?[8, 8];

        for (var row = 0; row < 8; row++)
            for (var column = 0; column < 8; column++)
                board[row, column] = position[row, column];

        var actual = board[startPoint.Y, startPoint.X];

        if (!actual.HasValue || actual.Value.PieceId != piece.PieceId || actual.Value.Type != piece.Type || actual.Value.Color != piece.Color)
            return false;

        piece = actual.Value;
        if (promotion.HasValue && (!Move.IsPromotionPiece(promotion.Value) || piece.Type != PieceType.Pawn ||
            endPoint.Y != (piece.Color == PlayerColor.White ? 7 : 0)))
            return false;
        var target = board[endPoint.Y, endPoint.X];

        if (target.HasValue && (target.Value.Color == piece.Color || target.Value.Type == PieceType.King))
            return false;

        var dx = endPoint.X - startPoint.X;
        var dy = endPoint.Y - startPoint.Y;
        var castling = piece.Type == PieceType.King && dy == 0 && Math.Abs(dx) == 2;

        if (castling)
        {
            if (!CanCastle(board, piece, startPoint, endPoint))
                return false;

            var rookColumn = dx > 0 ? 7 : 0;
            board[startPoint.Y, dx > 0 ? 5 : 3] = board[startPoint.Y, rookColumn];
            board[startPoint.Y, rookColumn] = null;
        }
        else if (piece.Type == PieceType.Pawn)
        {
            var direction = piece.Color == PlayerColor.White ? 1 : -1;
            var homeRow = piece.Color == PlayerColor.White ? 1 : 6;

            if (dx == 0)
            {
                if (target.HasValue || !(dy == direction || (dy == 2 * direction && startPoint.Y == homeRow && piece.MoveCount == 0 && !board[startPoint.Y + direction, startPoint.X].HasValue)))
                    return false;
            }
            else if (Math.Abs(dx) != 1 || dy != direction)
            {
                return false;
            }
            else if (!target.HasValue)
            {
                if (!position.CanEnPassant(piece, startPoint, endPoint)) return false;
                board[startPoint.Y, endPoint.X] = null;
            }
        }
        else if (!Attacks(board, piece, startPoint, endPoint))
        {
            return false;
        }

        board[startPoint.Y, startPoint.X] = null;
        board[endPoint.Y, endPoint.X] = piece;
        return KingIsSafe(board, piece.Color);
    }

    private static bool CanCastle(Piece?[,] board, Piece king, Point start, Point end)
    {
        var homeRow = king.Color == PlayerColor.White ? 0 : 7;

        if (king.MoveCount != 0 || start != new Point(4, homeRow) || (end != new Point(2, homeRow) && end != new Point(6, homeRow)))
            return false;
        
        var rookColumn = end.X == 6 ? 7 : 0;
        var rook = board[homeRow, rookColumn];
        
        if (rook is not { Type: PieceType.Rook } || rook.Value.Color != king.Color || rook.Value.MoveCount != 0)
            return false;

        var step = Math.Sign(end.X - start.X);
        
        for (var x = start.X + step; x != rookColumn; x += step)
            if (board[homeRow, x].HasValue)
                return false;

        if (!KingIsSafe(board, king.Color))
            return false;
        
        var crossing = (Piece?[,])board.Clone();
        crossing[start.Y, start.X] = null;
        crossing[homeRow, start.X + step] = king;
        return KingIsSafe(crossing, king.Color);
    }

    private static bool KingIsSafe(Piece?[,] board, PlayerColor color)
    {
        Point? king = null;
        for (var y = 0; y < 8; y++)
            for (var x = 0; x < 8; x++)
                if (board[y, x] is { } p && p.Color == color && p.Type == PieceType.King)
                {
                    if (king.HasValue)
                        return false;

                    king = new Point(x, y);
                }

        if (!king.HasValue)
            return false;

        for (var y = 0; y < 8; y++)
            for (var x = 0; x < 8; x++)
                if (board[y, x] is { } p && p.Color != color && Attacks(board, p, new Point(x, y), king.Value))
                    return false;

        return true;
    }

    private static bool Attacks(Piece?[,] board, Piece piece, Point start, Point end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;

        if (dx == 0 && dy == 0)
            return false;
        
        switch (piece.Type)
        {
            case PieceType.Pawn:
                return Math.Abs(dx) == 1 && dy == (piece.Color == PlayerColor.White ? 1 : -1);
            case PieceType.Knight:
                return Math.Abs(dx) * Math.Abs(dy) == 2;
            case PieceType.King:
                return Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1;
            case PieceType.Bishop:
                if (Math.Abs(dx) != Math.Abs(dy)) return false;
                break;
            case PieceType.Rook:
                if (dx != 0 && dy != 0) return false;
                break;
            case PieceType.Queen:
                if (dx != 0 && dy != 0 && Math.Abs(dx) != Math.Abs(dy)) return false;
                break;
            default:
                return false;
        }

        var stepX = Math.Sign(dx);
        var stepY = Math.Sign(dy);

        for (int x = start.X + stepX, y = start.Y + stepY; x != end.X || y != end.Y; x += stepX, y += stepY)
            if (board[y, x].HasValue)
                return false;

        return true;
    }

    private static bool Inside(Point point) =>
        point.X is >= 0 and < 8 && point.Y is >= 0 and < 8;
}
