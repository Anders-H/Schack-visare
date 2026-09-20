using System;
using System.Drawing;
using ChessEngine;
using ChessEngine.Moves;

var checks = 0;
Point Square(string s) => new(s[0] - 'a', s[1] - '1');
void Check(string name, bool expected, string from, string to, bool white = true, params string[] history)
{
    var board = new BoardData();
    var moves = new MoveList();
    // Some fixtures deliberately arrange positions using the viewer's permissive replay.
    foreach (var entry in history)
    {
        var start = Square(entry.Substring(0, 2));
        var end = Square(entry.Substring(2, 2));
        var p = board[start.Y, start.X]!.Value;
        var move = new Move(start, end, p.Type, p.Color, p.PieceId, moves.Count);
        moves.Add(move);
        board.ApplyMove(move);
    }
    var origin = Square(from);
    var piece = board[origin.Y, origin.X]!.Value;
    var result = new ChessRules(white, moves).IsMoveLegal(piece, origin, Square(to));
    if (result != expected) throw new Exception(name + ": got " + result);
    if (board[origin.Y, origin.X]!.Value.MoveCount != piece.MoveCount)
        throw new Exception("Validation mutated position");
    checks++;
}
Check("Pawn one step", true, "e2", "e3");
Check("Pawn two steps", true, "e2", "e4");
Check("Pawn three steps", false, "e2", "e5");
Check("Pawn diagonal empty", false, "e2", "f3");
Check("Wrong turn", false, "e7", "e5");
Check("Black pawn direction", true, "e7", "e5", false);
Check("Knight jumps", true, "g1", "f3");
Check("Knight invalid", false, "g1", "g3");
Check("Own capture", false, "e1", "e2");
Check("Blocked bishop", false, "c1", "h6");
Check("Blocked rook", false, "a1", "a3");
Check("Blocked queen", false, "d1", "h5");
Check("Same square", false, "e2", "e2");
Check("Off board", false, "e2", "e9");
Check("Pawn capture", true, "e4", "d5", true, "e2e4", "d7d5");
Check("Pawn forward capture", false, "e4", "e5", true, "e2e4", "e7e5");
Check("Open bishop", true, "f1", "b5", true, "e2e4");
Check("Castle blocked", false, "e1", "g1");
Check("Castle clear", true, "e1", "g1", true, "e2e4", "g1f3", "f1c4");
Check("Castle king moved", false, "e1", "g1", true, "e2e4", "g1f3", "f1c4", "e1e2", "e2e1");
Check("Castle rook moved", false, "e1", "g1", true, "e2e4", "g1f3", "f1c4", "h1g1", "g1h1");
Check("Castle through attack", false, "e1", "g1", true, "e2e4", "g1f3", "f1c4", "f2f4", "a8f3");
Check("Pinned piece", false, "e2", "f2", true, "e7e6", "a8e3");
Check("Ignore check", false, "a2", "a3", true, "e2e4", "e7e6", "a8e2");
Check("Capture checking rook", true, "e1", "e2", true, "e2e4", "e7e6", "a8e2");
Check("King enters pawn attack", false, "e1", "e2", true, "e2e4", "d7d3");
Check("Promotion", true, "a7", "b8", true, "a2a7");
Check("En passant", true, "e5", "d6", true, "e2e5", "d7d5");
Console.WriteLine($"Passed {checks} checks.");


Check("Expired en passant", false, "e5", "d6", true, "e2e5", "d7d5", "a2a3");
Check("One-step pawn cannot be captured en passant", false, "e5", "d6", true, "e2e5", "d7d6", "d6d5");
Check("En passant exposes king horizontally", false, "g5", "f6", true, "e1h5", "g2g5", "a8a5", "f7f5");
Check("En passant captures checking pawn", true, "e5", "d6", true, "e1e4", "e2e5", "d7d5");
Console.WriteLine($"Passed {checks} rule checks.");
BrainTests.Run();
