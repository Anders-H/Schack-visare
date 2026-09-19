#nullable enable
using System.Collections.Generic;
using ChessEngine.Pieces;

namespace ChessEngine.Moves;

public class MoveList : List<Move>
{
    public GamePosition? InitialPosition { get; set; }

    public bool IsGameEnded => Count > 0 && this[Count - 1].GameEnd != EndingType.MoveIsNotGameEnd;

    public bool IsWhitesTurn => Count == 0
        ? InitialPosition?.IsWhitesTurn ?? true
        : this[Count - 1].Color == PlayerColor.Black;
}
