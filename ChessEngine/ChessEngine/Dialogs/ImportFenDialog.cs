#nullable enable
using ChessEngine.ExternalParsers;
using System.Windows.Forms;

namespace ChessEngine.Dialogs;

public partial class ImportFenDialog : Form
{
    public string Contents { get; private set; }

    public ImportFenDialog()
    {
        Contents = "";
        InitializeComponent();
    }

    private void txtFenSource_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        btnOk.Enabled = false;
        var source = txtFenSource.Text.Trim();

        if (string.IsNullOrEmpty(source))
        {
            txtChessEngineFormat.Text = @"Source is empty.";
            return;
        }

        var parser = new FenParser(source);
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
            MessageBox.Show(this, @"No valid FEN content to import.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}