#nullable enable
using System;

namespace ChessEngine;

public class GameParserResult
{
    public bool Success { get; }
    public string GameName { get; }
    public DateTime GameDate { get; }
    public string WhitePlayerName { get; }
    public string BlackPlayerName { get; }
    public MoveList Moves { get; }
    public string Message { get; }

    public GameParserResult(bool success, string gameName, DateTime gameDate, string whitePlayerName, string blackPlayerName, MoveList moves, string message)
    {
        Success = success;
        GameName = gameName;
        GameDate = gameDate;
        WhitePlayerName = whitePlayerName;
        BlackPlayerName = blackPlayerName;
        Moves = moves;
        Message = message;
    }
}
