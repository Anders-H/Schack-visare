using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessEngine.Moves;

public static class ComputerChessMovePicker
{
    private static Random Rnd { get; }

    static ComputerChessMovePicker()
    {
        Rnd = new Random();
    }

    public static Move GetImpossibleMove(List<Move> legalMoves)
    {
        switch (legalMoves.Count)
        {
            case 2:
            case 3:
            case 4:
                return legalMoves.First();
            default:
                var x = Rnd.Next(0, 10);
                switch (x)
                {
                    case 7:
                    case 8:
                        return legalMoves[1];
                    case 9:
                        return legalMoves[2];
                    default:
                        return legalMoves[0];
                }
        }
    }

    public static Move GetBrutalMove(List<Move> legalMoves)
    {
        switch (legalMoves.Count)
        {
            case 2:
            case 3:
                return legalMoves.First();
            default:
                var max = 8;

                if (legalMoves.Count < max)
                    max = legalMoves.Count;

                return legalMoves[Rnd.Next(0, max)];
        }
    }

    public static Move GetChallengingMove(List<Move> legalMoves)
    {
        switch (legalMoves.Count)
        {
            case 2:
                return legalMoves.First();
            default:
                var max = 10;

                if (legalMoves.Count < max)
                    max = legalMoves.Count;

                return legalMoves[Rnd.Next(0, max)];
        }
    }

    public static Move GetModerateMove(List<Move> legalMoves)
    {
        var max = 11;

        if (legalMoves.Count < max)
            max = legalMoves.Count;

        return legalMoves[Rnd.Next(0, max)];
    }

    public static Move GetCasualMove(List<Move> legalMoves)
    {
        var max = legalMoves.Count;

        if (max > 10)
            max /= 2;

        return legalMoves[Rnd.Next(0, max)];
    }
}
