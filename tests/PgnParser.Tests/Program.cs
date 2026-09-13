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
Check("En passant", Header + "1.e4 a6 2.e5 d5 3.exd6 *", "E2-E4;A7-A6;E4-E5;D7-D5;E5-D6;");
Check("Promotion", Header + "1.e8=Q *", error: "capture notation");
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


var promotionPgn = Header + "1.a4 h5 2.a5 h4 3.a6 h3 4.axb7 hxg2 5.bxa8=N gxh1=R 6.Nxc7+ Qxc7 *";
Check("Both colors underpromote and promoted knight moves", promotionPgn,
    "A2-A4;H7-H5;A4-A5;H5-H4;A5-A6;H4-H3;A6-B7;H3-G2;B7-A8=N;G2-H1=R;A8-C7;D8-C7;");
Check("Black en passant", Header + "1.a3 e5 2.a4 e4 3.d4 exd3 *",
    "A2-A3;E7-E5;A3-A4;E5-E4;D2-D4;E4-D3;");
Check("Expired en passant", Header + "1.e4 a6 2.e5 d5 3.Nf3 a5 4.exd6 *", error: "no legal");
Check("Invalid promotion piece", Header + "1.e4=K *", error: "Q, R, B or N");
Check("Promotion on wrong rank", Header + "1.e4=Q *", error: "promotion");
Check("Promotion of knight", Header + "1.Nf3=Q *", error: "promotion");
Check("Missing PGN promotion choice", Header + "1.a4 h5 2.a5 h4 3.a6 h3 4.axb7 hxg2 5.bxa8 *", error: "promotion");

ChessEngine.BoardData Replay(string data)
{
    var parsed = new GameParser(data).Parse();
    if (!parsed.Success) throw new Exception(parsed.Message);
    var board = new BoardData();
    foreach (var move in parsed.Moves) board.ApplyMove(move);
    return board;
}
var ep = Replay("EP;2026-09-13;W;B;E2-E4;A7-A6;E4-E5;D7-D5;E5-D6;");
if (ep[4,3].HasValue || ep[4,4].HasValue || ep[5,3]?.Type != ChessEngine.Pieces.PieceType.Pawn)
    throw new Exception("En passant board");
count++;
const string promotionPrefix = "Promotion;2026-09-13;W;B;A2-A4;H7-H5;A4-A5;H5-H4;A5-A6;H4-H3;A6-B7;H3-G2;";
foreach (var symbol in new[] { "Q", "R", "B", "N" })
{
    var data = promotionPrefix + "B7-A8=" + symbol + ";G2-H1=" + symbol + ";";
    var board = Replay(data);
    var type = ChessEngine.Moves.Move.ParsePromotion(symbol[0]);
    if (board[7,0]?.Type != type || board[0,7]?.Type != type ||
        board[7,0]?.PieceId != 8 || board[0,7]?.PieceId != 31 || board[7,0]?.MoveCount != 5)
        throw new Exception("Promotion state " + symbol);
    var parsed = new GameParser(data).Parse();
    if (GameFileFormat.Serialize(parsed.GameName, parsed.GameDate, parsed.WhitePlayerName, parsed.BlackPlayerName, parsed.Moves) != data)
        throw new Exception("Promotion serialization");
    count++;
}
var legacy = Replay(promotionPrefix + "B7-A8;G2-H1;");
if (legacy[7,0]?.Type != ChessEngine.Pieces.PieceType.Queen || legacy[0,7]?.Type != ChessEngine.Pieces.PieceType.Queen)
    throw new Exception("Legacy promotion");
count++;
foreach (var malformed in new[] { "E7-E8=K", "E7-E8=P", "E7-E8=", "E2-E4=Q", "E7-E8=QQ" })
{
    if (new ChessEngine.Moves.MoveParser(malformed, 0).Parse().Success)
        throw new Exception("Accepted malformed promotion: " + malformed);
    count++;
}
Console.WriteLine($"Passed {count} PGN and special-move checks.");
