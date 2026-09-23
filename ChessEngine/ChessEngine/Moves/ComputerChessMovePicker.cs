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

    public static Move GetImpossibleMove(int moveIndex, List<Move> legalMoves)
    {
        switch (legalMoves.Count)
        {
            case 2:
            case 3:
            case 4:
            {
                if (moveIndex >= 3)
                    return legalMoves.First();

                var x = Rnd.Next(0, legalMoves.Count);
                return legalMoves[x];

            }
            default:
            {
                switch (moveIndex)
                {
                    case 0:
                    {
                        var max = 12;

                        if (legalMoves.Count < max)
                            max = legalMoves.Count;

                        var x = Rnd.Next(0, max);
                        return legalMoves[x];
                    }
                    case 1:
                    {
                        var max = 5;

                        if (legalMoves.Count < max)
                            max = legalMoves.Count;

                        var x = Rnd.Next(0, max);
                        return legalMoves[x];
                    }
                    case 2:
                    {
                        var max = 3;

                        if (legalMoves.Count < max)
                            max = legalMoves.Count;

                        var x = Rnd.Next(0, max);
                        return legalMoves[x];
                    }
                    default:
                    {
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
            }
        }
    }

    public static Move GetBrutalMove(int moveIndex, List<Move> legalMoves)
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

    public static Move GetChallengingMove(int moveIndex, List<Move> legalMoves)
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
