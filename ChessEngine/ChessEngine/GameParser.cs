#nullable enable
using System;
using System.Globalization;
using System.Text;

namespace ChessEngine;

public class GameParser
{
    private readonly string _source;

    public GameParser(string source)
    {
        _source = source;
    }

    public GameParserResult Parse()
    {
        var parts = _source.Split(';');
        
        if (parts.Length < 4)
            return new GameParserResult(false, "", DateTime.Today, "", "", [], "Invalid game data.");

        var s = new StringBuilder();
        var gameName = parts[0].Trim();
        var gameDateRaw = parts[1].Trim();
        var whitePlayerName = parts[2].Trim();
        var blackPlayerName = parts[3].Trim();
        var moves = new MoveList();
        DateTime gameDate;

        try
        {
            gameDate = DateTime.ParseExact(gameDateRaw, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch
        {
            s.AppendLine("Game date could not be parsed. ");
            gameDate = DateTime.Now;
        }

        if (parts.Length > 4)
        {
            var errors = 0;
            var moveNumber = 0;

            for (var i = 4; i < parts.Length; i++)
            {
                var moveSource = parts[i].Trim();

                if (string.IsNullOrWhiteSpace(moveSource))
                    continue;

                var moveParser = new MoveParser(moveSource, moveNumber);
                var parseResult = moveParser.Parse();

                if (parseResult.Success && parseResult.Move != null)
                {
                    moves.Add(parseResult.Move);
                }
                else
                {
                    s.AppendLine($"Move {moveNumber + 1} could not be parsed: {parseResult.Message}");
                    errors++;
                }

                moveNumber++;

                if (errors >= 3)
                    return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, moves, s.ToString().Trim());
            }
        }

        var gameData = new BoardData();
        var completedMoves = new MoveList();

        foreach (var parsedMove in moves)
        {
            var start = parsedMove.StartPoint;
            var end = parsedMove.EndPoint;
            var piece = gameData[start.Y, start.X];

            if (!piece.HasValue)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: no piece at {start}.");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            if (piece.Value.Color != parsedMove.Color)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: the piece at {start} has the wrong color.");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            var targetPiece = gameData[end.Y, end.X];

            if (targetPiece.HasValue && targetPiece.Value.Color == piece.Value.Color)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: a piece of the same color occupies {end}.");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            var completedMove = new Move(
                start,
                end,
                piece.Value.Type,
                piece.Value.Color,
                piece.Value.PieceId,
                parsedMove.MoveNumber);

            try
            {
                gameData.ApplyMove(completedMove);
            }
            catch (InvalidOperationException ex)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: {ex.Message}");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            completedMoves.Add(completedMove);
        }

        moves = completedMoves;
        return new GameParserResult(true, gameName, gameDate, whitePlayerName, blackPlayerName, moves, s.ToString().Trim());
    }
}
