using ChessEngine.Pieces;
using System;
using System.Windows.Forms;

namespace ChessEngine.Dialogs;

public partial class PromotePawnDialog : Form
{
    public bool IsWhitesTurn { private get; set; }

    public PieceType SelectedPieceType =>
        (PieceType)comboBox1.SelectedItem;
    
    public PromotePawnDialog()
    {
        InitializeComponent();
    }

    private void PromotePawnDialog_Load(object sender, EventArgs e)
    {
        Text = IsWhitesTurn ? "Promote White Pawn" : "Promote Black Pawn";
        comboBox1.Items.AddRange([PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight]);
        comboBox1.SelectedIndex = 0;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
    }
}
