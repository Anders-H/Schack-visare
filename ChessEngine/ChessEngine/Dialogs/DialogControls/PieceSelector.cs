#nullable enable
using System.ComponentModel;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ChessEngine.Pieces;

namespace ChessEngine.Dialogs.DialogControls;

public sealed class PieceSelector : ComboBox
{
    private static readonly Bitmap PieceSprites = Properties.Resources.pieces;

    // The constructor owns this fixed list; do not serialize it again in the designer.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ObjectCollection Items => base.Items;

    public PieceSelector()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawMode = DrawMode.OwnerDrawFixed;
        ItemHeight = 24;
        DropDownWidth = 110;
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
        var highlighted = (e.State & DrawItemState.Selected) != 0 && (e.State & DrawItemState.ComboBoxEdit) == 0;
        var background = highlighted ? SystemColors.Highlight : BackColor;
        var foreground = !Enabled ? SystemColors.GrayText : highlighted ? SystemColors.HighlightText : ForeColor;

        using (var brush = new SolidBrush(background))
        {
            e.Graphics.FillRectangle(brush, e.Bounds);
        }

        if (e.Index >= 0 && e.Index < Items.Count && Items[e.Index] is PieceAtSquare piece && piece != PieceAtSquare.None)
        {
            var white = piece <= PieceAtSquare.WhiteKing;
            var type = (PieceType)((int)piece - (int)(white ? PieceAtSquare.WhitePawn : PieceAtSquare.BlackPawn));
            // The black sprites run in reverse order in the shared sprite sheet.
            var spriteWidth = PieceSprites.Width / 6 - 2;
            var spriteHeight = PieceSprites.Height / 2 - 2;
            var column = white ? (int)type : 5 - (int)type;
            var source = new Rectangle(column * spriteWidth, white ? 0 : spriteHeight, spriteWidth, spriteHeight);
            var height = Math.Max(1, e.Bounds.Height - 2);
            var width = Math.Max(1, height * spriteWidth / spriteHeight);
            var destination = new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 1, width, height);
            var state = e.Graphics.Save();
            
            try
            {
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.DrawImage(PieceSprites, destination, source, GraphicsUnit.Pixel);
            }
            finally
            {
                e.Graphics.Restore(state);
            }

            var textBounds = Rectangle.FromLTRB(destination.Right + 2, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
            
            TextRenderer.DrawText(e.Graphics, type.ToString(), e.Font,
                textBounds, foreground, TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        e.DrawFocusRectangle();
        base.OnDrawItem(e);
    }
}
