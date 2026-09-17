using System;
using System.Drawing;
using ChessEngine;
using ChessEngine.ExternalParsers;
using ChessEngine.Moves;
using ChessEngine.Pieces;

var count = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    count++;
}
string Import(string fen)
{
    Check(new FenParser(fen).Parse(out var message, out var contents), "Import: " + message);
    Check(message == "" && contents.Length > 0, "Successful output");
    return contents;
}
GameParserResult Read(string contents)
{
    var result = new GameParser(contents).Parse();
    Check(result.Success, "Read: " + result.Message);
    return result;
}
BoardData Replay(GameParserResult game)
{
    var board = new BoardData(game.Moves.InitialPosition);
    foreach (var move in game.Moves) board.ApplyMove(move);
    return board;
}
bool Legal(GameParserResult game, Point start, Point end)
{
    var board = Replay(game);
    return new ChessRules(game.Moves.IsWhitesTurn, game.Moves).IsMoveLegal(board[start.Y, start.X]!.Value, start, end);
}

const string Initial = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
var initialData = Import(Initial);
var initial = Read(initialData);
var standard = new BoardData();
var imported = Replay(initial);
for (var y = 0; y < 8; y++)
    for (var x = 0; x < 8; x++)
        Check(standard[y, x]?.Type == imported[y, x]?.Type && standard[y, x]?.Color == imported[y, x]?.Color, "Board orientation and pieces");
Check(initial.Moves.Count == 0 && initial.Moves.IsWhitesTurn, "No invented history");
Check(Legal(initial, new Point(4, 1), new Point(4, 3)), "Initial pawn double step");
Check(Import("\uFEFF  " + Initial.Replace(" ", "\t") + "\r\n") == initialData, "Whitespace and BOM");

const string BlackFen = "4k3/8/8/8/8/8/8/4K3 b - - 17 42";
var blackData = Import(BlackFen);
var black = Read(blackData);
Check(!black.Moves.IsWhitesTurn && black.Moves.InitialPosition!.HalfmoveClock == 17 && black.Moves.InitialPosition.FullmoveNumber == 42, "Black turn and counters");
var continued = Read(blackData + "E8-D7;E1-D2;");
Check(continued.Moves[0].Color == PlayerColor.Black && continued.Moves[0].MoveNumber == 0 && continued.Moves[1].Color == PlayerColor.White, "Black starts without shifting replay indexes");
var saved = GameFileFormat.Serialize(continued.GameName, continued.GameDate, continued.WhitePlayerName, continued.BlackPlayerName, continued.Moves);
Check(saved == blackData + "E8-D7;E1-D2;", "Save preserves initial FEN and moves");
Check(Replay(Read(saved))[6, 3] is { Type: PieceType.King, Color: PlayerColor.Black }, "Reopen continued game");
Check(Replay(black)[7, 4] is { Type: PieceType.King }, "Replay does not mutate initial board");

const string Castles = "r3k2r/8/8/8/8/8/8/R3K2R";
var castles = Read(Import(Castles + " w KQkq - 0 1"));
Check(Legal(castles, new Point(4, 0), new Point(6, 0)) && Legal(castles, new Point(4, 0), new Point(2, 0)), "White castling rights");
var limited = Read(Import(Castles + " w Q - 0 1"));
Check(!Legal(limited, new Point(4, 0), new Point(6, 0)) && Legal(limited, new Point(4, 0), new Point(2, 0)), "Independent castling rights");
var noRights = Read(Import(Castles + " w - - 0 1"));
Check(!Legal(noRights, new Point(4, 0), new Point(2, 0)), "No implicit castling rights");
var blackCastles = Read(Import(Castles + " b kq - 0 12"));
Check(Legal(blackCastles, new Point(4, 7), new Point(2, 7)) && Legal(blackCastles, new Point(4, 7), new Point(6, 7)), "Black castling rights");
var castled = Replay(Read(Import(Castles + " b kq - 0 12") + "E8-C8;"));
Check(castled[7, 2] is { Type: PieceType.King } && castled[7, 3] is { Type: PieceType.Rook } && castled[7, 0] == null, "Imported castling replay");

var epData = Import("4k3/8/8/3pP3/8/8/8/4K3 w - d6 0 2");
var ep = Read(epData);
Check(Legal(ep, new Point(4, 4), new Point(3, 5)), "White en passant immediately after import");
var captured = Replay(Read(epData + "E5-D6;"));
Check(captured[4, 3] == null && captured[5, 3] is { Type: PieceType.Pawn, Color: PlayerColor.White }, "En passant replay");
var expired = Read(epData + "E1-D1;E8-D8;");
Check(!Legal(expired, new Point(4, 4), new Point(3, 5)), "Imported en passant expires");
var blackEpData = Import("4k3/8/8/8/3Pp3/8/8/4K3 b - d3 0 2");
Check(Legal(Read(blackEpData), new Point(4, 3), new Point(3, 2)), "Black en passant immediately after import");
Check(Replay(Read(blackEpData + "E4-D3;"))[3, 3] == null, "Black en passant replay");
Import("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"); // No adjacent capturer is required.
var promoted = Replay(Read(Import("4k3/8/8/8/8/8/p7/4K3 b - - 0 7") + "A2-A1=N;"));
Check(promoted[0, 0] is { Type: PieceType.Knight, Color: PlayerColor.Black }, "Black promotion on first move");

foreach (var invalid in new[]
{
    "", " ", "8/8/8/8/8/8/8/8 w - - 0 1", Initial + " extra",
    "4k3/8/8/8/8/8/4K3 w - - 0 1", BlackFen.Replace("4k3", "4k4"),
    BlackFen.Replace("4k3", "4k2"), BlackFen.Replace("4k3", "4x3"),
    BlackFen.Replace("4k3", "4k0"), BlackFen.Replace("4k3", "4k9"),
    BlackFen.Replace("4K3", "3KK3"), BlackFen.Replace("4K3", "P3K3"),
    BlackFen.Replace(" b ", " W "), BlackFen.Replace(" b - ", " b K "),
    Castles + " w KK - 0 1", Castles + " w qK - 0 1", Castles + " w K- - 0 1",
    Castles + " w A - 0 1", BlackFen.Replace("17 42", "-1 42"),
    BlackFen.Replace("17 42", "1 0"), BlackFen.Replace("17 42", "+1 42"),
    BlackFen.Replace("17 42", "1 2147483648"), BlackFen.Replace("17 42", "2147483648 1"),
    "4k3/8/8/3pP3/8/8/8/4K3 w - d3 0 2", "4k3/8/8/4P3/8/8/8/4K3 w - d6 0 2",
    "4k3/8/8/3pP3/8/8/8/4K3 w - d6 1 2"
})
{
    Check(!new FenParser(invalid).Parse(out var message, out var contents) && message.Length > 0 && contents == "", "Reject invalid FEN: " + invalid);
}
Check(!new FenParser(null!).Parse(out _, out _), "Null input");
Check(!new GameParser("Test;2026-09-17;?;?;FEN invalid;").Parse().Success, "Malformed saved FEN");
Check(Read("Legacy;2026-09-17;White;Black;E2-E4;").Moves.InitialPosition == null, "Legacy format unchanged");
Console.WriteLine($"Passed {count} FEN checks.");
