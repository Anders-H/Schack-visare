#nullable enable
using System.Windows.Forms;

namespace ChessEngine.Dialogs.DialogControls;

public sealed class PieceSelector : ComboBox
{
    public PieceSelector()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        Items.Add(PieceAtSquare.None);
        Items.Add(PieceAtSquare.WhitePawn);
        Items.Add(PieceAtSquare.WhiteRook);
        Items.Add(PieceAtSquare.WhiteKnight);
        Items.Add(PieceAtSquare.WhiteBishop);
        Items.Add(PieceAtSquare.WhiteQueen);
        Items.Add(PieceAtSquare.WhiteKing);
        Items.Add(PieceAtSquare.BlackPawn);
        Items.Add(PieceAtSquare.BlackRook);
        Items.Add(PieceAtSquare.BlackKnight);
        Items.Add(PieceAtSquare.BlackBishop);
        Items.Add(PieceAtSquare.BlackQueen);
        Items.Add(PieceAtSquare.BlackKing);
        SelectedIndex = 0;
    }
}
