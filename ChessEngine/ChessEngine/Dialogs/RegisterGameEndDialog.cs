#nullable enable
using System;
using System.Windows.Forms;
using ChessEngine.Moves;

namespace ChessEngine.Dialogs;

public partial class RegisterGameEndDialog : Form
{
    public int MoveNumber { private get; set; }
    public Move? ChessMove { get; private set; }

    public RegisterGameEndDialog()
    {
        InitializeComponent();
    }

    private void RegisterGameEndDialog_Load(object sender, EventArgs e)
    {
        comboBox1.Items.Add("White wins");
        comboBox1.Items.Add("Black wins");
        comboBox1.Items.Add("Draw");
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnOk.Enabled = comboBox1.SelectedIndex > -1;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        if (comboBox1.SelectedIndex < 0)
            return;

        ChessMove = comboBox1.SelectedIndex switch
        {
            0 => new Move(RegisterEndingType.WhiteWins, MoveNumber),
            1 => new Move(RegisterEndingType.BlackWins, MoveNumber),
            2 => new Move(RegisterEndingType.Draw, MoveNumber),
            _ => throw new InvalidOperationException("Invalid selection")
        };

        DialogResult = DialogResult.OK;
    }
}
