#nullable enable
using System.Windows.Forms;
using ChessEngine.ExternalParsers;

namespace ChessEngine.Dialogs;

public partial class ImportPgnDialog : Form
{
    public string Contents { get; private set; }

    public ImportPgnDialog()
    {
        Contents = "";
        InitializeComponent();
    }

    private void txtPgnSource_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        btnOk.Enabled = false;
        var source = txtPgnSource.Text.Trim();

        if (string.IsNullOrEmpty(source))
        {
            txtChessEngineFormat.Text = @"Source is empty.";
            return;
        }

        var parser = new PgnParser(source);
        var result = parser.Parse(out var message, out var contents);

        if (result)
        {
            Contents = contents;
            txtChessEngineFormat.Text = contents;
            btnOk.Enabled = true;
            return;
        }

        txtChessEngineFormat.Text = message;
    }

    private void btnOk_Click(object sender, System.EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Contents))
        {
            MessageBox.Show(this, @"No valid PGN content to import.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
