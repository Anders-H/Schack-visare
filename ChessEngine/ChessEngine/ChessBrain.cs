#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ChessEngine.Moves;
using ChessEngine.Pieces;

namespace ChessEngine;

public class ChessBrain
{
    public const int CheckmateScore = 100000;
    private static readonly PieceType[] Promotions = [PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight];
    private readonly BoardData _board;
    private readonly MoveList _moves;

    /// <param name="board">The position after the supplied move history.</param>
    /// <param name="moves">History including the initial position and side to move.</param>
    public ChessBrain(BoardData board, MoveList moves)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));
        _moves = moves ?? throw new ArgumentNullException(nameof(moves));
    }

    /// <summary>
    /// Returns legal moves, best score first, without changing the board or history.
    /// Scores evaluate the position for the moving player after the opponent's best reply.
    /// This shallow heuristic does not detect repetition or the fifty-move rule.
    /// </summary>
    public List<Move> GetLegalMoves()
    {
        if (_moves.IsGameEnded)
            return [];

        var isWhitesTurn = _moves.IsWhitesTurn;
        var color = isWhitesTurn ? PlayerColor.White : PlayerColor.Black;
        var opponent = color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        var position = _board.Copy();
        var moves = GenerateMoves(position, color, _moves.Count).ToList();

        foreach (var move in moves)
        {
            var next = position.Copy();
            next.ApplyMove(move);
            var givesCheck = !ChessRules.KingIsSafe(next.CopyBoard(), opponent);
            var replies = GenerateMoves(next, opponent, move.MoveNumber + 1).ToList();

            if (replies.Count == 0)
            {
                move.Score = givesCheck ? CheckmateScore : 0; // Stalemate is a draw.
                continue;
            }

            // Assume the opponent chooses the reply that is worst for us, including recaptures.
            var bonus = givesCheck ? 40 : 0;

            if (move.Piece == PieceType.King && Math.Abs(move.EndPoint.X - move.StartPoint.X) == 2)
                bonus += 25;

            var worstReply = int.MaxValue;

            foreach (var reply in replies)
            {
                var afterReply = next.Copy();
                afterReply.ApplyMove(reply);
                var score = Evaluate(afterReply, color) + bonus;

                if (!GenerateMoves(afterReply, color, reply.MoveNumber + 1).Any())
                    score = ChessRules.KingIsSafe(afterReply.CopyBoard(), color) ? 0 : -CheckmateScore;

                worstReply = Math.Min(worstReply, score);
            }

            move.Score = worstReply;
        }

        return moves.OrderByDescending(move => move.Score).ToList();
    }

    private IEnumerable<Move> GenerateMoves(BoardData board, PlayerColor color, int moveNumber)
    {
        var rules = new ChessRules(color == PlayerColor.White, _moves);

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                if (board[y, x] is not { } piece || piece.Color != color)
                    continue;

                var start = new Point(x, y);
                for (var endY = 0; endY < 8; endY++)
                {
                    for (var endX = 0; endX < 8; endX++)
                    {
                        var end = new Point(endX, endY);

                        if (!rules.IsMoveLegal(board, piece, start, end))
                            continue;

                        if (piece.Type == PieceType.Pawn && endY == (color == PlayerColor.White ? 7 : 0))
                        {
                            foreach (var promotion in Promotions)
                                yield return new Move(start, end, piece, moveNumber, promotion);
                        }
                        else
                        {
                            yield return new Move(start, end, piece, moveNumber);
                        }
                    }
                }
            }
        }
    }

    private static int Evaluate(BoardData position, PlayerColor color)
    {
        var board = position.CopyBoard();
        var score = 0;

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                if (board[y, x] is not { } piece)
                    continue;
                
                var value = Value(piece.Type);
                var defended = false;
                var attacked = false;
                var square = new Point(x, y);

                for (var otherY = 0; otherY < 8; otherY++)
                {
                    for (var otherX = 0; otherX < 8; otherX++)
                    {
                        if (board[otherY, otherX] is { } other && ChessRules.Attacks(board, other, new Point(otherX, otherY), square))
                        {
                            if (other.Color == piece.Color)
                                defended = true;
                            else
                                attacked = true;
                        }
                    }
                }

                // Geometric protection is approximate: a pinned defender may not be able to move.
                var pieceScore = value;
                if (piece.Type != PieceType.King)
                {
                    if (defended)
                        pieceScore += value / 20;

                    if (attacked)
                        pieceScore -= value / (defended ? 5 : 2);

                    if (piece.Type is PieceType.Knight or PieceType.Bishop or PieceType.Pawn)
                        pieceScore += 4 * (Math.Min(x, 7 - x) + Math.Min(y, 7 - y));
                }
                else if (attacked)
                {
                    pieceScore -= 40;
                }

                score += piece.Color == color ? pieceScore : -pieceScore;
            }
        }

        return score;
    }

    private static int Value(PieceType type) => type switch
    {
        PieceType.Pawn => 100,
        PieceType.Knight => 320,
        PieceType.Bishop => 330,
        PieceType.Rook => 500,
        PieceType.Queen => 900,
        _ => 0
    };
}
