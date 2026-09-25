#nullable enable
using System;
using ChessEngine.Dialogs.DialogControls;
using System.Windows.Forms;

namespace ChessEngine.Dialogs;

public partial class ConfigureBoardDialog : Form
{
    public ConfigureBoardDialog()
    {
        InitializeComponent();
    }

    private void ConfigureBoardDialog_Load(object sender, EventArgs e)
    {
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var c = GetComboBox(x, y);
                c.BackColor = (x + y) % 2 == 0
                    ? System.Drawing.Color.FromArgb(255, 240, 217)
                    : System.Drawing.Color.FromArgb(181, 136, 99);
            }
        }
    }

    public void SetBoardData(PieceAtSquare[,] boardData)
    {
        
    }

    private PieceSelector GetComboBox(int x, int y)
    {
        switch (y)
        {
            case 0:
                return x switch
                {
                    0 => pieceSelector1,
                    1 => pieceSelector2,
                    2 => pieceSelector3,
                    3 => pieceSelector4,
                    4 => pieceSelector5,
                    5 => pieceSelector6,
                    6 => pieceSelector7,
                    7 => pieceSelector8,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 1:
                return x switch
                {
                    0 => pieceSelector16,
                    1 => pieceSelector15,
                    2 => pieceSelector14,
                    3 => pieceSelector13,
                    4 => pieceSelector12,
                    5 => pieceSelector11,
                    6 => pieceSelector10,
                    7 => pieceSelector9,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 2:
                return x switch
                {
                    0 => pieceSelector24,
                    1 => pieceSelector23,
                    2 => pieceSelector22,
                    3 => pieceSelector21,
                    4 => pieceSelector20,
                    5 => pieceSelector19,
                    6 => pieceSelector18,
                    7 => pieceSelector17,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 3:
                return x switch
                {
                    0 => pieceSelector32,
                    1 => pieceSelector31,
                    2 => pieceSelector30,
                    3 => pieceSelector29,
                    4 => pieceSelector28,
                    5 => pieceSelector27,
                    6 => pieceSelector26,
                    7 => pieceSelector25,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 4:
                return x switch
                {
                    0 => pieceSelector64,
                    1 => pieceSelector63,
                    2 => pieceSelector62,
                    3 => pieceSelector61,
                    4 => pieceSelector60,
                    5 => pieceSelector59,
                    6 => pieceSelector58,
                    7 => pieceSelector57,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 5:
                return x switch
                {
                    0 => pieceSelector56,
                    1 => pieceSelector55,
                    2 => pieceSelector54,
                    3 => pieceSelector53,
                    4 => pieceSelector52,
                    5 => pieceSelector51,
                    6 => pieceSelector50,
                    7 => pieceSelector49,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 6:
                return x switch
                {
                    0 => pieceSelector48,
                    1 => pieceSelector47,
                    2 => pieceSelector46,
                    3 => pieceSelector45,
                    4 => pieceSelector44,
                    5 => pieceSelector43,
                    6 => pieceSelector42,
                    7 => pieceSelector41,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 7:
                return x switch
                {
                    0 => pieceSelector40,
                    1 => pieceSelector39,
                    2 => pieceSelector38,
                    3 => pieceSelector37,
                    4 => pieceSelector36,
                    5 => pieceSelector35,
                    6 => pieceSelector34,
                    7 => pieceSelector33,
                    _ => throw new ArgumentOutOfRangeException()
                };
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
