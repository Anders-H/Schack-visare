#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using ChessEngine.Moves;
using ChessEngine.Pieces;

namespace ChessEngine.ExternalParsers;

public class PgnParser
{
    private readonly string _source;

    public PgnParser(string source) { _source = source; }

    public bool Parse(out string message, out string contents)
    {
        message = "";
        contents = "";

        try
        {
            if (string.IsNullOrWhiteSpace(_source))
                throw new FormatException("PGN source is empty.");

            var tags = new Dictionary<string, string>(StringComparer.Ordinal);
            var board = new BoardData();
            var moves = new MoveList();
            var started = false;
            var ended = false;
            string? result = null;

            foreach (var token in Tokenize(_source))
            {
                if (token.StartsWith("[", StringComparison.Ordinal))
                {
                    if (started)
                        throw new FormatException("Only one PGN game can be imported at a time.");

                    var tag = Regex.Match(token, "^\\[\\s*(\\w+)\\s+\"((?:[^\"\\\\]|\\\\[\"\\\\])*)\"\\s*\\]$");
                    
                    if (!tag.Success || tags.ContainsKey(tag.Groups[1].Value))
                        throw new FormatException("Invalid or duplicate PGN tag: " + token);
                    
                    tags.Add(tag.Groups[1].Value, Regex.Replace(tag.Groups[2].Value, @"\\([""\\])", "$1"));
                    continue;
                }

                var san = Regex.Replace(token, @"^\d+\.+", "");
                
                if (san.Length == 0 || Regex.IsMatch(san, @"^\.+$") || Regex.IsMatch(san, @"^\$\d+$"))
                    continue;
                
                if (ended)
                    throw new FormatException("Unexpected content after the result; import one game at a time.");
                
                started = true;
                
                if (tags.ContainsKey("FEN") || (tags.TryGetValue("SetUp", out var setup) && setup != "0"))
                    throw new FormatException("Custom starting positions (FEN/SetUp) are not supported by ChessEngine.");
                
                if (san == "1-0" || san == "0-1" || san == "1/2-1/2" || san == "*")
                {
                    ended = true;
                    result = san;
                    continue;
                }
                
                var move = ParseMove(san, board, moves);
                board.ApplyMove(move);
                moves.Add(move);
            }

            if (!started)
                throw new FormatException("PGN contains no moves or game result.");
            
            if (result != null && tags.TryGetValue("Result", out var declaredResult) && declaredResult != result)
                throw new FormatException("The Result tag does not match the movetext result.");
            
            string Tag(string name) => tags.TryGetValue(name, out var value) ? value : "?";
            // The destination requires a complete date; report any substituted components.
            var dateText = Tag("Date");

            if (dateText == "?")
                dateText = "????.??.??";
            
            if (!Regex.IsMatch(dateText, @"^(\d{4}|\?{4})\.(\d{2}|\?{2})\.(\d{2}|\?{2})$") || !DateTime.TryParseExact(dateText.Replace("????", "0001").Replace("??", "01"), "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                throw new FormatException("Invalid PGN Date tag; expected yyyy.MM.dd.");
            
            var nameParts = new List<string>();
            if (Tag("Event") != "?")
                nameParts.Add(Tag("Event"));

            if (Tag("Site") != "?")
                nameParts.Add(Tag("Site"));
            
            var name = nameParts.Count == 0 ? "Imported PGN game" : string.Join(" - ", nameParts);
            contents = GameFileFormat.Serialize(name, date, Tag("White"), Tag("Black"), moves);

            if (dateText.Contains("?"))
                message = "Unknown date components were replaced with year 0001, month 01 or day 01.";
            
            return true;
        }
        catch (FormatException ex)
        {
            message = ex.Message;
            return false;
        }
    }

    private static Move ParseMove(string token, BoardData board, MoveList moves)
    {
        FormatException Error(string reason) => new($"Move {moves.Count / 2 + 1}{(moves.Count % 2 == 0 ? "." : "...")} {token}: {reason}");
        var san = Regex.Replace(token, @"[+#?!]+$", "");
        
        if (san.Contains("="))
            throw Error("promotion is not supported by ChessEngine.");
        
        var color = moves.Count % 2 == 0 ? PlayerColor.White : PlayerColor.Black;
        var rules = new ChessRules(color == PlayerColor.White, moves);
        var castle = san.Replace('0', 'O');
        
        if (castle == "O-O" || castle == "O-O-O")
        {
            var start = new Point(4, color == PlayerColor.White ? 0 : 7);
            var end = new Point(castle == "O-O" ? 6 : 2, start.Y);
            var king = board[start.Y, start.X];
            
            if (!king.HasValue || king.Value.Type != PieceType.King || !rules.IsMoveLegal(king.Value, start, end))
                throw Error("illegal castling.");
            
            return new Move(start, end, king.Value.Type, color, king.Value.PieceId, moves.Count);
        }

        var match = Regex.Match(san, @"^(?<piece>[KQRBN])?(?<file>[a-h])?(?<rank>[1-8])?(?<capture>x)?(?<end>[a-h][1-8])$");
        
        if (!match.Success)
            throw Error("invalid SAN notation.");
        
        var type = match.Groups["piece"].Value switch
        {
            "K" => PieceType.King, "Q" => PieceType.Queen, "R" => PieceType.Rook,
            "B" => PieceType.Bishop, "N" => PieceType.Knight, _ => PieceType.Pawn
        };

        var file = match.Groups["file"].Value;
        var rank = match.Groups["rank"].Value;
        var capture = match.Groups["capture"].Success;
        
        if (type == PieceType.Pawn && (rank.Length != 0 || (capture ? file.Length != 1 : file.Length != 0)))
            throw Error("invalid pawn notation.");
        
        var square = match.Groups["end"].Value;
        var target = new Point(square[0] - 'a', square[1] - '1');
        
        if (type == PieceType.Pawn && (target.Y == 0 || target.Y == 7))
            throw Error("promotion is not supported by ChessEngine.");
        
        if (capture && !board[target.Y, target.X].HasValue && type == PieceType.Pawn)
            throw Error("capture on an empty square; en passant is not supported by ChessEngine.");
        
        if (capture != board[target.Y, target.X].HasValue)
            throw Error("capture notation does not match the position.");
        
        Move? candidate = null;

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var piece = board[y, x];

                if (!piece.HasValue || piece.Value.Color != color || piece.Value.Type != type ||
                    (file.Length > 0 && x != file[0] - 'a') || (rank.Length > 0 && y != rank[0] - '1')) continue;
                
                var start = new Point(x, y);
                
                if (!rules.IsMoveLegal(piece.Value, start, target))
                    continue;
                
                if (candidate != null)
                    throw Error("ambiguous move; specify the origin file or rank.");
                
                candidate = new Move(start, target, type, color, piece.Value.PieceId, moves.Count);
            }
        }

        return candidate ?? throw Error("no legal move matches this notation.");
    }

    private static IEnumerable<string> Tokenize(string source)
    {
        var depth = 0;

        for (var i = 0; i < source.Length;)
        {
            var c = source[i];

            if (char.IsWhiteSpace(c) || c == '\uFEFF')
            {
                i++;
                continue;
            }

            if (c == ';' || (c == '%' && (i == 0 || source[i - 1] == '\n' || source[i - 1] == '\r')))
            {
                while (i < source.Length && source[i] != '\n' && source[i] != '\r') i++;
                continue;
            }

            if (c == '{')
            {
                var end = source.IndexOf('}', i + 1);

                if (end < 0)
                    throw new FormatException("Unterminated PGN comment.");
                
                i = end + 1;
                continue;
            }

            if (c == '(')
            {
                depth++;
                i++;
                continue;
            }

            if (c == ')')
            {
                if (depth == 0) throw new FormatException("Unexpected closing variation parenthesis.");
                depth--; i++; continue;
            }

            if (depth > 0)
            {
                i++;
                continue;
            }

            if (c == '[')
            {
                var start = i++;
                var quoted = false;
                
                for (; i < source.Length; i++)
                {
                    if (quoted && source[i] == '\\')
                    {
                        i++;
                        continue;
                    }

                    if (source[i] == '"')
                        quoted = !quoted;

                    if (!quoted && source[i] == ']')
                        break;
                }

                if (i >= source.Length)
                    throw new FormatException("Unterminated PGN tag.");

                yield return source.Substring(start, ++i - start);
                continue;
            }

            if (c == '}' || c == ']')
                throw new FormatException("Unexpected PGN delimiter.");
            
            var tokenStart = i++;

            while (i < source.Length && !char.IsWhiteSpace(source[i]) && "{}();[]$".IndexOf(source[i]) < 0)
                i++;
            
            yield return source.Substring(tokenStart, i - tokenStart);
        }

        if (depth != 0)
            throw new FormatException("Unterminated PGN variation.");
    }
}
