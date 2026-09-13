using System;
using ChessEngine;
using ChessEngine.ExternalParsers;

var count = 0;
const string Header = "[Event \"Test\"] [Site \"Stockholm\"] [Date \"2026.09.13\"] [White \"Alice\"] [Black \"Bob\"] ";
void Check(string name, string pgn, string? expectedMoves = null, string? error = null)
{
    var success = new PgnParser(pgn).Parse(out var message, out var contents);
    if (error != null)
    {
        if (success || contents != "" || !message.Contains(error, StringComparison.OrdinalIgnoreCase))
            throw new Exception(name + ": expected error " + error + ", got " + message);
    }
    else
    {
        var expected = "Test - Stockholm;2026-09-13;Alice;Bob;" + expectedMoves;
        if (!success || contents != expected) throw new Exception(name + ": " + message + " / " + contents);
        var parsed = new GameParser(contents).Parse();
        if (!parsed.Success) throw new Exception(name + ": round trip failed");
    }
    count++;
}
Check("Opening and castling", Header + "1.e4 e5 2.Nf3 Nc6 3.Bb5 a6 4.Ba4 Nf6 5.O-O *",
    "E2-E4;E7-E5;G1-F3;B8-C6;F1-B5;A7-A6;B5-A4;G8-F6;E1-G1;");
Check("Comments and nested variations", Header + "1.e4{hello}(1.d4 (1.c4) d5) e5$1 2.Nf3!? ; comment\n2...Nc6 *",
    "E2-E4;E7-E5;G1-F3;B8-C6;");
Check("Capture", Header + "1.e4 d5 2.exd5 Qxd5 *", "E2-E4;D7-D5;E4-D5;D8-D5;");
Check("Mate suffix", Header + "1.f3 e5 2.g4 Qh4# 0-1", "F2-F3;E7-E5;G2-G4;D8-H4;");
Check("File disambiguation", Header + "1.Nf3 Nf6 2.d3 d6 3.Nbd2 *", "G1-F3;G8-F6;D2-D3;D7-D6;B1-D2;");
Check("Ambiguous", Header + "1.Nf3 Nf6 2.d3 d6 3.Nd2 *", error: "ambiguous");
Check("Illegal", Header + "1.e5 *", error: "no legal");
Check("Missing capture marker", Header + "1.e4 d5 2.ed5 *", error: "pawn notation");
Check("En passant", Header + "1.e4 a6 2.e5 d5 3.exd6 *", error: "en passant");
Check("Promotion", Header + "1.e8=Q *", error: "promotion");
Check("FEN", Header + "[SetUp \"1\"] [FEN \"anything\"] *", error: "starting positions");
Check("Multiple games", Header + "1.e4 * " + Header + "*", error: "one PGN game");
Check("Unclosed comment", Header + "1.e4 {oops", error: "Unterminated");
Check("Unclosed variation", Header + "1.e4 (1.d4", error: "Unterminated");
Check("Unclosed tag", "[Event \"oops", error: "Unterminated");
Check("Bad date", Header.Replace("2026.09.13", "2026.02.30") + "*", error: "Date");
Check("Unsafe metadata", Header.Replace("Alice", "A;B") + "*", error: "semicolons");
Check("Conflicting result", Header + "[Result \"1-0\"] 1.e4 0-1", error: "Result");
Check("Empty", "", error: "empty");
Check("Queenside castling", Header + "1.d4 d5 2.Nc3 Nc6 3.Bf4 Bf5 4.Qd2 Qd7 5.O-O-O O-O-O *",
    "D2-D4;D7-D5;B1-C3;B8-C6;C1-F4;C8-F5;D1-D2;D8-D7;E1-C1;E8-C8;");
var partial = new PgnParser(Header.Replace("2026.09.13", "2026.??.??") + "*");
if (!partial.Parse(out var warning, out var output) || !output.Contains(";2026-01-01;") || warning.Length == 0)
    throw new Exception("Partial date");
count++;
Console.WriteLine($"Passed {count} PGN checks.");
