#nullable enable
namespace ChessEngine;

public class MoveParserResult
{
    public bool Success { get; }
    public Move? Move { get; }
    public string Message { get; }

    public MoveParserResult(bool success, Move? move, string message)
    {
        Success = success;
        Move = move;
        Message = message;
    }
}
