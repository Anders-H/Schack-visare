using System;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;
using ChessEngine.Pieces;

namespace ChessEngine.MainWindowControllers;

public class GameFileController
{
    private readonly MainWindow _owner;

    public GameFileController(MainWindow owner)
    {
        _owner = owner;
    }

    public void UserSaveAs(string filename, string gameName, BoardControl boardControl, ToolStripStatusLabel statusLabel)
    {
        using var dialog = new SaveFileDialog();
        dialog.AddExtension = true;
        dialog.DefaultExt = "txt";
        dialog.FileName = string.IsNullOrWhiteSpace(filename) ? "game.txt" : Path.GetFileName(filename);
        dialog.Filter = @"Chess game files (*.txt)|*.txt|All files (*.*)|*.*";
        dialog.OverwritePrompt = true;
        dialog.RestoreDirectory = true;
        dialog.Title = @"Save chess game";

        if (dialog.ShowDialog(_owner) == DialogResult.OK)
            SaveGame(dialog.FileName, gameName, boardControl, statusLabel);
    }

    public void UserSave(string filename, string gameName, BoardControl boardControl, ToolStripStatusLabel statusLabel)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            UserSaveAs(filename, gameName, boardControl, statusLabel);
            return;
        }

        SaveGame(filename, gameName, boardControl, statusLabel);
    }

    private void SaveGame(string filename, string gameName, BoardControl boardControl, ToolStripStatusLabel statusLabel)
    {
        if (!CheckGame(out var errorMessage))
        {
            var m = $@"The game is not in a valid state, and will not be able to load again. {errorMessage} Do you want to save it anyway?";

            if (MessageBox.Show(_owner, m, _owner.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
        }

        try
        {
            var contents = GameFileFormat.Serialize(
                gameName,
                boardControl.GameDate,
                boardControl.WhitePlayerName,
                boardControl.BlackPlayerName,
                _owner.Moves);

            File.WriteAllText(filename, contents, new UTF8Encoding(false));
            _owner.Filename = filename;
            statusLabel.Text = $@"Saved {Path.GetFileName(filename)}.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException or FormatException)
        {
            MessageBox.Show(
                _owner,
                $@"The game could not be saved.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                _owner.Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private bool CheckGame(out string errorMessage)
    {
        for (var index = 0; index < _owner.Moves.Count; index++)
        {
            var expectedColor = index % 2 == 0
                ? PlayerColor.White
                : PlayerColor.Black;
            var move = _owner.Moves[index];

            if (move.Color == expectedColor)
                continue;

            errorMessage = $@"Move {index + 1} ({move}) is registered for {move.Color}, but {expectedColor} must make this move.";
            return false;
        }

        errorMessage = "";
        return true;
    }
}
