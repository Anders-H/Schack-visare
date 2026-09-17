#nullable enable
using System;
using System.Drawing;
using System.Globalization;
using ChessEngine.Moves;
using ChessEngine.Pieces;

namespace ChessEngine.ExternalParsers;

public class FenParser
{
    private readonly string _source;
    public FenParser(string source) => _source = source;

    public bool Parse(out string message, out string contents)
    {
        message = "";
        contents = "";

        try
        {
            var moves = new MoveList { InitialPosition = ParsePosition() };
            contents = GameFileFormat.Serialize("Imported FEN position", DateTime.MinValue, "?", "?", moves);
            return true;
        }
        catch (FormatException ex)
        {
            message = ex.Message;
            return false;
        }
    }

    public GamePosition ParsePosition()
    {
        if (string.IsNullOrWhiteSpace(_source))
            throw new FormatException("FEN source is empty.");
        
        var fields = _source.Trim().TrimStart('\uFEFF').Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        
        if (fields.Length != 6)
            throw new FormatException("FEN must contain exactly six fields.");
        
        var ranks = fields[0].Split('/');
        
        if (ranks.Length != 8)
            throw new FormatException("FEN piece placement must contain eight ranks.");
        
        var board = new Piece?[8, 8];
        var kings = new int[2];
        var id = 0;
        
        for (var rank = 0; rank < 8; rank++)
        {
            var file = 0;

            foreach (var symbol in ranks[rank])
            {
                if (symbol >= '1' && symbol <= '8')
                {
                    file += symbol - '0';
                    continue;
                }
                
                var type = symbol switch
                {
                    'P' or 'p' => PieceType.Pawn, 'N' or 'n' => PieceType.Knight,
                    'B' or 'b' => PieceType.Bishop, 'R' or 'r' => PieceType.Rook,
                    'Q' or 'q' => PieceType.Queen, 'K' or 'k' => PieceType.King,
                    _ => throw new FormatException("Invalid FEN piece: " + symbol)
                };

                if (file >= 8)
                    throw new FormatException("Each FEN rank must contain exactly eight squares.");
                
                var white = char.IsUpper(symbol);
                
                if (type == PieceType.King)
                    kings[white ? 0 : 1]++;
                
                if (type == PieceType.Pawn && rank is 0 or 7)
                    throw new FormatException("A FEN pawn cannot occupy the first or last rank.");
                
                board[7 - rank, file++] = new Piece(id++, type, white ? PlayerColor.White : PlayerColor.Black)
                {
                    // Unknown history must never grant castling rights.
                    MoveCount = type is PieceType.King or PieceType.Rook ? 1 : 0
                };
            }
            if (file != 8)
                throw new FormatException("Each FEN rank must contain exactly eight squares.");
        }

        if (kings[0] != 1 || kings[1] != 1)
            throw new FormatException("FEN must contain exactly one king of each color.");
        
        if (fields[1] != "w" && fields[1] != "b")
            throw new FormatException("FEN active color must be w or b.");
        
        if (fields[2] != "-")
        {
            var previous = -1;
            
            foreach (var right in fields[2])
            {
                var index = "KQkq".IndexOf(right);

                if (index < 0 || index <= previous)
                    throw new FormatException("Invalid FEN castling rights; expected KQkq in order or -.");
                
                previous = index;
                var row = index < 2 ? 0 : 7;
                var column = index % 2 == 0 ? 7 : 0;
                var color = index < 2 ? PlayerColor.White : PlayerColor.Black;
                var king = board[row, 4];
                var rook = board[row, column];
                
                if (king is not { Type: PieceType.King } || king.Value.Color != color || rook is not { Type: PieceType.Rook } || rook.Value.Color != color)
                    throw new FormatException("FEN castling rights require the king and rook on their home squares.");
                
                var unmovedKing = king.Value;
                var unmovedRook = rook.Value;
                unmovedKing.MoveCount = unmovedRook.MoveCount = 0;
                board[row, 4] = unmovedKing;
                board[row, column] = unmovedRook;
            }
        }

        Point? enPassant = null;
        
        if (fields[3] != "-")
        {
            var square = fields[3];
            var white = fields[1] == "w";

            if (square.Length != 2 || square[0] < 'a' || square[0] > 'h' || square[1] != (white ? '6' : '3'))
                throw new FormatException("Invalid FEN en passant target for the active color.");
            
            var target = new Point(square[0] - 'a', square[1] - '1');
            var direction = white ? 1 : -1;
            var pawn = board[target.Y - direction, target.X];
            
            if (board[target.Y, target.X].HasValue || board[target.Y + direction, target.X].HasValue || pawn is not { Type: PieceType.Pawn } || pawn.Value.Color == (white ? PlayerColor.White : PlayerColor.Black))
                throw new FormatException("FEN en passant target does not match a double pawn move.");
            
            enPassant = target;
        }
        if (!int.TryParse(fields[4], NumberStyles.None, CultureInfo.InvariantCulture, out var halfmove))
            throw new FormatException("FEN halfmove clock must be a non-negative integer within Int32 range.");

        if (!int.TryParse(fields[5], NumberStyles.None, CultureInfo.InvariantCulture, out var fullmove) || fullmove < 1)
            throw new FormatException("FEN fullmove number must be a positive integer within Int32 range.");

        if (enPassant.HasValue && halfmove != 0)
            throw new FormatException("FEN halfmove clock must be zero after a double pawn move.");

        return new GamePosition(board, string.Join(" ", fields), fields[1] == "w", enPassant, halfmove, fullmove);
    }
}
