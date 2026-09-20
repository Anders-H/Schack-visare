using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ChessEngine;
using ChessEngine.Moves;
using ChessEngine.Pieces;

static class BrainTests
{
    private static int checks;
    private static Point Square(string square) => new(square[0] - 'a', square[1] - '1');
    private static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception(name);
        checks++;
    }

    private static (BoardData board, MoveList history) Position(bool white, params (string square, PieceType type, PlayerColor color)[] pieces)
    {
        var board = new Piece?[8, 8];
        for (var i = 0; i < pieces.Length; i++)
        {
            var square = Square(pieces[i].square);
            board[square.Y, square.X] = new Piece(i, pieces[i].type, pieces[i].color);
        }
        var position = new GamePosition(board, "", white, null, 0, 1);
        return (new BoardData(position), new MoveList { InitialPosition = position });
    }

    public static void Run()
    {
        var watch = Stopwatch.StartNew();
        var board = new BoardData();
        var history = new MoveList();
        var brain = new ChessBrain(board, history);
        var initial = brain.GetLegalMoves();
        Check(initial.Count == 20 && initial.All(m => m.Color == PlayerColor.White), "White initial moves");
        Check(initial.Select(m => m.Score).SequenceEqual(initial.Select(m => m.Score).OrderByDescending(s => s)), "Sorted scores");
        Check(initial.All(m => m.MoveNumber == 0 && m.PieceId.HasValue), "Move metadata");
        Check(history.Count == 0 && board[1, 4]!.Value.MoveCount == 0 && board.DeadPieces.Count == 0, "Search preserves input");
        Check(initial.Select(m => (m.ToString(), m.Score)).SequenceEqual(brain.GetLegalMoves().Select(m => (m.ToString(), m.Score))), "Repeatable search");
        var e4 = initial.Single(m => m.StartPoint == Square("e2") && m.EndPoint == Square("e4"));
        board.ApplyMove(e4);
        history.Add(e4);
        var blackMoves = brain.GetLegalMoves();
        Check(blackMoves.Count == 20 && blackMoves.All(m => m.Color == PlayerColor.Black && m.MoveNumber == 1), "Black replies");
        history.Add(new Move(RegisterEndingType.Draw, 1));
        Check(brain.GetLegalMoves().Count == 0, "Recorded game ending");

        const PlayerColor W = PlayerColor.White, B = PlayerColor.Black;
        const PieceType K = PieceType.King, Q = PieceType.Queen, R = PieceType.Rook, P = PieceType.Pawn;
        var black = Position(false, ("a1", K, W), ("h8", K, B), ("e7", P, B));
        Check(new ChessBrain(black.board, black.history).GetLegalMoves().All(m => m.Color == B), "Initial position side to move");

        var promotion = Position(true, ("h1", K, W), ("h8", K, B), ("a7", P, W));
        var promotions = new ChessBrain(promotion.board, promotion.history).GetLegalMoves().Where(m => m.StartPoint == Square("a7")).ToList();
        Check(promotions.Count == 4 && promotions.Select(m => m.Promotion).Distinct().Count() == 4, "All four promotions");
        Check(promotions.First().Promotion == Q, "Promotion material value");

        var castle = Position(true, ("e1", K, W), ("a1", R, W), ("h1", R, W), ("e8", K, B));
        var castles = new ChessBrain(castle.board, castle.history).GetLegalMoves();
        Check(castles.Any(m => m.ToString() == "E1-G1") && castles.Any(m => m.ToString() == "E1-C1"), "Both castlings");

        var ep = Position(false, ("a1", K, W), ("h8", K, B), ("e5", P, W), ("d7", P, B));
        var doubleStep = new Move(Square("d7"), Square("d5"), ep.board[6, 3]!.Value, 0);
        ep.board.ApplyMove(doubleStep);
        ep.history.Add(doubleStep);
        var epMoves = new ChessBrain(ep.board, ep.history).GetLegalMoves();
        Check(epMoves.Any(m => m.ToString() == "E5-D6"), "En passant generated");
        Check(ep.board.CanEnPassant(ep.board[4, 4]!.Value, Square("e5"), Square("d6")) && ep.board.DeadPieces.Count == 0, "En passant state preserved");

        var pinned = Position(true, ("e1", K, W), ("e2", R, W), ("e8", R, B), ("h8", K, B));
        Check(!new ChessBrain(pinned.board, pinned.history).GetLegalMoves().Any(m => m.ToString() == "E2-F2"), "Pinned move excluded");

        var capture = Position(true, ("a1", K, W), ("h8", K, B), ("d1", R, W), ("d5", Q, B));
        Check(new ChessBrain(capture.board, capture.history).GetLegalMoves().First().ToString() == "D1-D5", "Free queen preferred");
        var blackCapture = Position(false, ("a8", K, B), ("h1", K, W), ("d8", R, B), ("d4", Q, W));
        var bestBlack = new ChessBrain(blackCapture.board, blackCapture.history).GetLegalMoves().First();
        Check(bestBlack.ToString() == "D8-D4" && bestBlack.Score > 0, "Black scores use black perspective");
        var poison = Position(true, ("g1", K, W), ("h8", K, B), ("d1", Q, W), ("d5", P, B), ("d8", R, B));
        var poisonMoves = new ChessBrain(poison.board, poison.history).GetLegalMoves();
        Check(poisonMoves.First().Score > poisonMoves.Single(m => m.ToString() == "D1-D5").Score, "Recapture makes poisoned pawn unattractive");

        var mate = Position(true, ("f6", K, W), ("g6", Q, W), ("h8", K, B));
        var matingMoves = new ChessBrain(mate.board, mate.history).GetLegalMoves();
        Check(matingMoves.First().Score == ChessBrain.CheckmateScore && matingMoves.Single(m => m.ToString() == "G6-G7").Score == ChessBrain.CheckmateScore, "Mate ranked highest");
        Check(matingMoves.Single(m => m.ToString() == "G6-F7").Score == 0, "Stalemate scored as draw");
        var mated = Position(false, ("f6", K, W), ("g7", Q, W), ("h8", K, B));
        Check(new ChessBrain(mated.board, mated.history).GetLegalMoves().Count == 0, "Checkmated side has no moves");
        var stalemate = Position(false, ("f6", K, W), ("f7", Q, W), ("h8", K, B));
        Check(new ChessBrain(stalemate.board, stalemate.history).GetLegalMoves().Count == 0, "Stalemated side has no moves");
        Console.WriteLine($"Passed {checks} brain checks in {watch.ElapsedMilliseconds} ms.");
    }
}
