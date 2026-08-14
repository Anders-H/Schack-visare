#nullable enable
namespace ChessEngine;

public class BoardData
{
    private static readonly PieceType[] BackRank =
    {
        PieceType.Rook,
        PieceType.Knight,
        PieceType.Bishop,
        PieceType.Queen,
        PieceType.King,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook
    };

    private readonly Piece?[,] _board;

    public BoardData()
    {
        _board = new Piece?[8, 8];

        InitializeBackRank(0, PlayerColor.White, 0);
        InitializePawnRank(1, PlayerColor.White, 8);
        InitializeBackRank(7, PlayerColor.Black, 16);
        InitializePawnRank(6, PlayerColor.Black, 24);
    }

    private void InitializeBackRank(int row, PlayerColor color, int firstPieceId)
    {
        for (var column = 0; column < BackRank.Length; column++)
        {
            _board[row, column] = new Piece(
                firstPieceId + column,
                BackRank[column],
                color);
        }
    }

    private void InitializePawnRank(int row, PlayerColor color, int firstPieceId)
    {
        for (var column = 0; column < 8; column++)
        {
            _board[row, column] = new Piece(
                firstPieceId + column,
                PieceType.Pawn,
                color);
        }
    }
}
