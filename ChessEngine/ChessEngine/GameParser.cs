#nullable enable
using System;
using System.Globalization;
using System.Text;
using ChessEngine.Moves;
using ChessEngine.ExternalParsers;

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

        var firstMove = 4;
        if (parts.Length > 4 && parts[4].Trim().StartsWith("FEN ", StringComparison.Ordinal))
        {
            try
            {
                moves.InitialPosition = new FenParser(parts[4].Trim().Substring(4)).ParsePosition();
                firstMove = 5;
            }
            catch (FormatException ex)
            {
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, moves, ex.Message);
            }
        }

        if (parts.Length > firstMove)
        {
            var moveNumber = 0;

            for (var i = firstMove; i < parts.Length; i++)
            {
                var moveSource = parts[i].Trim();

                if (string.IsNullOrWhiteSpace(moveSource))
                    continue;

                var moveParser = new MoveParser(moveSource, moveNumber, moves.InitialPosition?.IsWhitesTurn ?? true);
                var parseResult = moveParser.Parse();

                if (parseResult.Success && parseResult.Move != null)
                {
                    moves.Add(parseResult.Move);
                }
                else
                {
                    s.AppendLine($"Move {moveNumber + 1} could not be parsed: {parseResult.Message}");
                    // Skipping a malformed move would corrupt turn order and special-move history.
                    return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, moves, s.ToString().Trim());
                }

                moveNumber++;

            }
        }

        var gameData = new BoardData(moves.InitialPosition);
        var completedMoves = new MoveList { InitialPosition = moves.InitialPosition };

        foreach (var parsedMove in moves)
        {
            var start = parsedMove.StartPoint;
            var end = parsedMove.EndPoint;
            var piece = gameData[start.Y, start.X];

            if (!piece.HasValue)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: no piece at {Move.FormatSquare(start)}.");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            if (piece.Value.Color != parsedMove.Color)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: the piece at {Move.FormatSquare(start)} has the wrong color.");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            var targetPiece = gameData[end.Y, end.X];

            if (targetPiece.HasValue && targetPiece.Value.Color == piece.Value.Color)
            {
                s.AppendLine($"Move {parsedMove.MoveNumber + 1} could not be applied: a piece of the same color occupies {Move.FormatSquare(end)}.");
                return new GameParserResult(false, gameName, gameDate, whitePlayerName, blackPlayerName, completedMoves, s.ToString().Trim());
            }

            var completedMove = new Move(start, end, piece.Value, parsedMove.MoveNumber, parsedMove.Promotion);

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
