#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChessEngine.Moves;

namespace ChessEngine.Dialogs;

public partial class GamePropertiesDialog : Form
{
    public string GameTitle { get; set; }
    public DateTime GameDate { get; set; }
    public string WhitePlayerName { get; set; }
    public string BlackPlayerName { get; set; }
    public MoveList Moves { get; set; }

    public GamePropertiesDialog()
    {
        GameTitle = "";
        GameDate = DateTime.Now;
        WhitePlayerName = "";
        BlackPlayerName = "";
        Moves = [];
        InitializeComponent();
    }

    private void GamePropertiesDialog_Load(object sender, EventArgs e)
    {
        txtGameTitle.Text = GameTitle.Trim();
        txtGameDate.Text = GameDate.ToString("yyyy-MM-dd");
        txtWhitePlayerName.Text = WhitePlayerName.Trim();
        txtBlackPlayerName.Text = BlackPlayerName.Trim();
        var moves = Moves.Count;
        var end = EndingType.MoveIsNotGameEnd;

        if (moves > 0 && Moves.Last().GameEnd != EndingType.MoveIsNotGameEnd)
        {
            moves--;
            end = Moves.Last().GameEnd;
        }

        txtMovesCount.Text = moves.ToString();
        txtGameEnding.Text = end switch
        {
            EndingType.WhiteWins => "White wins",
            EndingType.BlackWins => "Black wins",
            EndingType.Draw => "Draw",
            _ => "Game is not finished"
        };
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        if (!txtGameDate.Text.ParseDate(out var gameDate))
        {
            MessageBox.Show(this, @"Invalid game date.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        GameTitle = txtGameTitle.Text.Trim();
        GameDate = gameDate;
        WhitePlayerName = txtWhitePlayerName.Text.Trim();
        BlackPlayerName = txtBlackPlayerName.Text.Trim();
        DialogResult = DialogResult.OK;
    }

    private void txtGameTitle_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        txtGameTitle.Text = txtGameTitle.Text.Trim();
    }

    private void txtGameDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (txtGameDate.Text.ParseDate(out var gameDate))
        {
            lblGameDate.ForeColor = SystemColors.ControlText;
            txtGameDate.Text = gameDate.ToString("yyyy-MM-dd");
        }
        else
        {
            lblGameDate.ForeColor = Color.Red;
            txtGameDate.Text = txtGameDate.Text.Trim();
        }
    }

    private void txtWhitePlayerName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        txtWhitePlayerName.Text = txtWhitePlayerName.Text.Trim();
    }

    private void txtBlackPlayerName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        txtBlackPlayerName.Text = txtBlackPlayerName.Text.Trim();
    }
}
