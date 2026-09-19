#nullable enable
using System.Collections.Generic;
using ChessEngine.Pieces;

namespace ChessEngine.Moves;

public class MoveList : List<Move>
{
    public GamePosition? InitialPosition { get; set; }

    public bool IsWhitesTurn => Count == 0
        ? InitialPosition?.IsWhitesTurn ?? true
        : this[Count - 1].Color == PlayerColor.Black;
}
