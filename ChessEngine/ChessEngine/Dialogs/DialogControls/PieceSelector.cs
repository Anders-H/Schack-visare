#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ChessEngine.Dialogs.DialogControls;

public sealed class PieceSelector : ComboBox
{
    // The constructor owns this fixed list; do not serialize it again in the designer.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ObjectCollection Items => base.Items;

    public PieceSelector()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawMode = DrawMode.OwnerDrawFixed;
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

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        // Keep the square color in the closed control, even when it has focus.
        var highlighted = (e.State & DrawItemState.Selected) != 0
            && (e.State & DrawItemState.ComboBoxEdit) == 0;
        var background = highlighted ? SystemColors.Highlight : BackColor;
        var foreground = !Enabled ? SystemColors.GrayText
            : highlighted ? SystemColors.HighlightText : ForeColor;

        using (var brush = new SolidBrush(background))
            e.Graphics.FillRectangle(brush, e.Bounds);

        if (e.Index >= 0 && e.Index < Items.Count)
            TextRenderer.DrawText(e.Graphics, GetItemText(Items[e.Index]), e.Font,
                e.Bounds, foreground, TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.NoPrefix);

        e.DrawFocusRectangle();
        base.OnDrawItem(e);
    }
}
