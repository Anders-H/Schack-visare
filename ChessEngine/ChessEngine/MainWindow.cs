#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;
using ChessEngine.Dialogs;
using ChessEngine.Events;
using ChessEngine.MainWindowControllers;
using ChessEngine.Moves;
using ChessEngine.Pieces;

namespace ChessEngine;

public partial class MainWindow : Form
{
    private const int PlaybackIntervalMilliseconds = 300;
    private readonly Timer _playbackTimer = new();
    private readonly Font _boldMoveListFont;
    private bool _registerMoveMode;
    private bool _archonView;
    private readonly List<Piece> _graveyard;
    private ComputerPlayerSkill _computerLevelSkill;
    public MoveList Moves { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        _graveyard = [];
        _boldMoveListFont = new Font(listView1.Font, listView1.Font.Style | FontStyle.Bold);
        _playbackTimer.Interval = PlaybackIntervalMilliseconds;
        _playbackTimer.Tick += PlaybackTimer_Tick;
        boardControl1.MoveSelected += boardControl1_MoveSelected;
        boardControl1.MoveDragStarted += boardControl1_MoveDragStarted;
        boardControl1.MoveDragCancelled += cancelRegisterMoveToolStripMenuItem_Click;
        boardControl1.Paint += boardControl1_Paint;
        _registerMoveMode = false;
        Moves = [];
        CurrentMove = -1;
        Filename = "";
        GameName = "";
        _archonView = false;
        _computerLevelSkill = ComputerPlayerSkill.Challenging;
        ResizeBoard();
        UpdateControls();
    }

    public bool ViewFromBlacksPerspective =>
        fromBlacksPerspectiveToolStripMenuItem.Checked;

    public int CurrentMove
    {
        get;
        private set
        {
            field = value;
            boardControl1.SetSelectedMove(value >= 0 && value < Moves.Count ? Moves[value] : null);
            UpdateStatus();
        }
    }

    public string Filename
    {
        get;
        set
        {
            field = value;
            Text = string.IsNullOrWhiteSpace(value) ? @"Chess Engine" : $@"Chess Engine - [{value}]";
        }
    }

    private void UpdateStatus()
    {
        if (_registerMoveMode)
        {
            var turn = Moves.IsWhitesTurn ? "white" : "black";

            lblStatus.Text = $@"Storing move {Moves.Count + 1}, {turn}.";
        }
        else if (_playbackTimer.Enabled)
        {
            lblStatus.Text = $@"Playing move {CurrentMove + 1} of {Moves.Count}.";
        }
        else
        {
            lblStatus.Text = $@"Move {CurrentMove + 1} of {Moves.Count}.";
        }
    }

    private static string Version
    {
        get
        {
            var version = Application.ProductVersion ?? "";

            if (version.IndexOf('.') <= -1)
                return version;

            var temp = version.Split('.');
            version = $@"{temp[0]}.{temp[1]}";

            return version;
        }
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var response = MessageBox.Show(this, $@"Chess Engine version {Version} written by Anders Hesselbom. Application icon created by Vivek Kale.

Open version history?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (response != DialogResult.Yes)
            return;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://ahesselbom.se/chess/versionhistory.html",
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show(this, @"Failed to open version history.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(this, $@"Send a mail to anders@winsoft.se with subject ""Chess Engine {Version} bug report"". Would you like to open your email client?", @"Report a bug", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                var subject = Uri.EscapeDataString($"Chess Engine {Version} bug report");

                Process.Start(new ProcessStartInfo
                {
                    FileName = $"mailto:anders@winsoft.se?subject={subject}",
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show(this, @"Failed to open email client.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void MainWindow_Resize(object sender, EventArgs e) =>
        ResizeBoard();

    private void ResizeBoard()
    {
        var boardSize = panel1.Height > panel1.Width ? panel1.Width : panel1.Height;
        boardSize -= 4;
        var x = panel1.Width / 2 - boardSize / 2;
        var y = panel1.Height / 2 - boardSize / 2;
        boardControl1.Bounds = new Rectangle(x, y, boardSize, boardSize);
    }

    private void boardControl1_MoveDragStarted(object sender, CancelEventArgs e)
    {
        if (Moves.IsGameEnded || _playbackTimer.Enabled)
            return;

        registerMoveToolStripMenuItem_Click(sender, EventArgs.Empty);
        e.Cancel = false;
    }

    private void registerMoveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Moves.IsGameEnded)
            return;

        StopPlayback();

        if (CurrentMove != Moves.Count - 1)
            GoToMove(Moves.Count - 1);

        _registerMoveMode = true;
        boardControl1.BeginMoveRegistration();
        UpdateControls();
        UpdateStatus();
    }

    private void cancelRegisterMoveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _registerMoveMode = false;
        boardControl1.CancelMoveRegistration();
        UpdateControls();
        UpdateStatus();
    }

    private void boardControl1_MoveSelected(object sender, MoveSelectedEventArgs e)
    {
        if (!_registerMoveMode || Moves.IsGameEnded)
            return;

        var movedPiece = boardControl1.GetPieceAt(e.StartPoint.X, e.StartPoint.Y);

        if (!movedPiece.HasValue)
            throw new InvalidOperationException($@"No piece at {e.StartPoint}.");

        if (!CheckMove(e.Piece, e.StartPoint, e.EndPoint, out var errorMessage))
        {
            const string baseMessage = "The move does not seem to be valid";
            var message = string.IsNullOrWhiteSpace(errorMessage) ? $"{baseMessage}." : $@"{baseMessage}: {errorMessage}";

            if (MessageBox.Show(this, $@"{message}

Are you sure you want to save this move?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                cancelRegisterMoveToolStripMenuItem_Click(sender, EventArgs.Empty);
                return;
            }
        }

        var moveIndex = Moves.Count;
        PieceType? promotion = null;

        if (movedPiece.Value.Type == PieceType.Pawn && e.EndPoint.Y == (movedPiece.Value.Color == PlayerColor.White ? 7 : 0))
        {
            using var dialog = new PromotePawnDialog();
            dialog.IsWhitesTurn = movedPiece.Value.Color == PlayerColor.White;

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                cancelRegisterMoveToolStripMenuItem_Click(sender, EventArgs.Empty);
                return;
            }

            promotion = dialog.SelectedPieceType;
        }

        Moves.Add(new Move(e.StartPoint, e.EndPoint, movedPiece.Value, moveIndex, promotion));
        _registerMoveMode = false;
        boardControl1.CancelMoveRegistration();
        RenderMoveList();

        try
        {
            GoToMove(moveIndex);
        }
        catch (Exception exception)
        {
            listView1.Items.RemoveAt(listView1.Items.Count - 1);
            Moves.RemoveAt(Moves.Count - 1);
            lastToolStripMenuItem_Click(sender, e);
            MessageBox.Show(this, exception.Message, @"This move cannot be stored", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool CheckMove(Piece piece, Point startPoint, Point endPoint, out string errorMessage)
    {
        errorMessage = "";
        var whiteTurn = Moves.IsWhitesTurn;

        if (whiteTurn && piece.Color != PlayerColor.White)
        {
            errorMessage = "It is white's turn to move.";
            return false;
        }

        if (!whiteTurn && piece.Color != PlayerColor.Black)
        {
            errorMessage = "It is black's turn to move.";
            return false;
        }

        if (!MoveIsLegal(piece, startPoint, endPoint))
        {
            errorMessage = "The move is not legal according to the rules of Chess.";
            return false;
        }

        return true;
    }

    private bool MoveIsLegal(Piece piece, Point startPoint, Point endPoint)
    {
        var whiteTurn = Moves.IsWhitesTurn;

        var chessRules = new ChessRules(whiteTurn, Moves);
        return chessRules.IsMoveLegal(piece, startPoint, endPoint);
    }

    private void btnRegistrera_Click(object sender, EventArgs e) =>
        registerMoveToolStripMenuItem_Click(sender, e);

    private void btnAvbrytRegistrering_Click(object sender, EventArgs e) =>
        cancelRegisterMoveToolStripMenuItem_Click(sender, e);

    private void btnFirst_Click(object sender, EventArgs e) =>
        firstToolStripMenuItem_Click(sender, e);

    private void btnPrevious_Click(object sender, EventArgs e) =>
        previousToolStripMenuItem_Click(sender, e);

    private void btnPlay_Click(object sender, EventArgs e) =>
        playToolStripMenuItem_Click(sender, e);

    private void btnPause_Click(object sender, EventArgs e) =>
        pauseToolStripMenuItem_Click(sender, e);

    private void btnNext_Click(object sender, EventArgs e) =>
        nextToolStripMenuItem_Click(sender, e);

    private void btnLast_Click(object sender, EventArgs e) =>
        lastToolStripMenuItem_Click(sender, e);

    private void firstToolStripMenuItem_Click(object sender, EventArgs e)
    {
        StopPlayback();
        GoToMove(-1);

        if (listView1.Items.Count > 0)
            listView1.Items[0].EnsureVisible();
    }

    private void previousToolStripMenuItem_Click(object sender, EventArgs e)
    {
        StopPlayback();
        GoToMove(CurrentMove - 1);
    }

    private void playToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_registerMoveMode || Moves.Count == 0 || _playbackTimer.Enabled)
            return;

        if (CurrentMove >= Moves.Count - 1)
            GoToMove(-1);

        _playbackTimer.Start();
        UpdateControls();
        UpdateStatus();
    }

    private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        StopPlayback();
    }

    private void nextToolStripMenuItem_Click(object sender, EventArgs e)
    {
        StopPlayback();
        GoToMove(CurrentMove + 1);
    }

    private void lastToolStripMenuItem_Click(object sender, EventArgs e)
    {
        StopPlayback();
        GoToMove(Moves.Count - 1);
    }

    private void PlaybackTimer_Tick(object sender, EventArgs e)
    {
        if (CurrentMove < Moves.Count - 1)
            GoToMove(CurrentMove + 1);

        if (CurrentMove >= Moves.Count - 1)
            StopPlayback();
    }

    private void StopPlayback()
    {
        _playbackTimer.Stop();
        UpdateControls();
        UpdateStatus();
    }

    private void GoToMove(int moveIndex)
    {
        var lastMoveIndex = Moves.Count - 1;
        var targetMoveIndex = Math.Max(-1, Math.Min(moveIndex, lastMoveIndex));
        var position = new BoardData(Moves.InitialPosition);

        for (var index = 0; index <= targetMoveIndex; index++)
            position.ApplyMove(Moves[index]);

        boardControl1.SetPosition(position);
        // Rebuild from the displayed position, including when rewinding or loading a game.
        _graveyard.Clear();
        _graveyard.AddRange(position.DeadPieces);
        CurrentMove = targetMoveIndex;
        UpdateControls();
        UpdateSelectedPieceProperties();


        if (targetMoveIndex == -1 && listView1.Items.Count > 0)
            SelectInMoveList(0);
        else if (targetMoveIndex < 0)
            SelectNoneInMoveList();
        else
            SelectInMoveList(targetMoveIndex + 1);

        boardControl1.CalculateCoverage();
        boardControl1.Invalidate();
    }

    private void UpdateControls()
    {
        var isPlaying = _playbackTimer.Enabled;
        var navigationEnabled = !_registerMoveMode && !isPlaying;
        var canMoveBackward = navigationEnabled && CurrentMove >= 0;
        var canMoveForward = navigationEnabled && CurrentMove < Moves.Count - 1;

        firstToolStripMenuItem.Enabled = canMoveBackward;
        btnFirst.Enabled = canMoveBackward;
        previousToolStripMenuItem.Enabled = canMoveBackward;
        btnPrevious.Enabled = canMoveBackward;
        nextToolStripMenuItem.Enabled = canMoveForward;
        btnNext.Enabled = canMoveForward;
        lastToolStripMenuItem.Enabled = canMoveForward;
        btnLast.Enabled = canMoveForward;

        var canRegister = !_registerMoveMode && !isPlaying && !Moves.IsGameEnded;
        registerMoveToolStripMenuItem.Enabled = canRegister;
        btnRegistrera.Enabled = canRegister;
        registerComputerMoveToolStripMenuItem.Enabled = canRegister;
        btnRegisterComputerMove.Enabled = canRegister;
        registerGameEndingToolStripMenuItem.Enabled = canRegister;
        deleteLastMoveToolStripMenuItem.Enabled = Moves.Count > 0;
        cancelRegisterMoveToolStripMenuItem.Enabled = _registerMoveMode;
        btnAvbrytRegistrering.Enabled = _registerMoveMode;

        var canPlay = !_registerMoveMode && !isPlaying && Moves.Count > 0;
        playToolStripMenuItem.Enabled = canPlay;
        btnPlay.Enabled = canPlay;
        pauseToolStripMenuItem.Enabled = isPlaying;
        btnPause.Enabled = isPlaying;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _playbackTimer.Stop();
        _playbackTimer.Dispose();
        _boldMoveListFont.Dispose();
        base.OnFormClosed(e);
    }

    private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(this, @"Are you sure you want to create a new game? Any unsaved changes will be lost.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        _playbackTimer.Enabled = false;
        _registerMoveMode = false;
        Moves = [];
        RenderMoveList();
        GoToMove(-1);
        Filename = "";
        GameName = "";
    }

    private string? GameName
    {
        get => (field ?? "").Trim();
        set
        {
            field = (value ?? "").Trim();

            if (!string.IsNullOrWhiteSpace(field))
            {
                lblGameTitle.Text = field;
                return;
            }

            var w = boardControl1.WhitePlayerName;

            if (string.IsNullOrWhiteSpace(w))
                w = "White";

            var b = boardControl1.BlackPlayerName;

            if (string.IsNullOrWhiteSpace(b))
                b = "Black";

            var result = $"{w} vs {b} {DateTime.Now:yyyy-MM-dd}";
            lblGameTitle.Text = result;
        }
    }

    private void btnNewGame_Click(object sender, EventArgs e) =>
        newGameToolStripMenuItem_Click(sender, e);

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(this, @"Are you sure you want to open a game? Any unsaved changes will be lost.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        using var dialog = new OpenFileDialog();
        dialog.CheckFileExists = true;
        dialog.CheckPathExists = true;
        dialog.DefaultExt = "txt";
        dialog.Filter = @"Chess game files (*.txt)|*.txt|All files (*.*)|*.*";
        dialog.Multiselect = false;
        dialog.RestoreDirectory = true;
        dialog.Title = @"Open chess game";

        if (!string.IsNullOrWhiteSpace(Filename))
            dialog.FileName = Filename;

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var contents = File.ReadAllText(dialog.FileName, new UTF8Encoding(false, true));
            var parser = new GameParser(contents);
            var result = parser.Parse();

            if (result.Success)
            {
                _playbackTimer.Enabled = false;
                _registerMoveMode = false;
                GameName = result.GameName;
                boardControl1.GameDate = result.GameDate;
                boardControl1.WhitePlayerName = result.WhitePlayerName;
                boardControl1.BlackPlayerName = result.BlackPlayerName;
                Moves = result.Moves;
                Filename = dialog.FileName;
                RenderMoveList();
                GoToMove(-1);
                var message = result.Message.Trim();

                if (!string.IsNullOrWhiteSpace(message))
                    MessageBox.Show(
                        this,
                        message,
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                if (listView1.Items.Count > 0)
                    listView1.Items[0].EnsureVisible();
            }
            else
            {
                MessageBox.Show(this, result.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblStatus.Text = $@"Read {Path.GetFileName(dialog.FileName)} ({contents.Length} characters).";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            MessageBox.Show(this, $@"The game could not be opened.{Environment.NewLine}{Environment.NewLine}{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnOpen_Click(object sender, EventArgs e) =>
        openToolStripMenuItem_Click(sender, e);

    private void saveToolStripMenuItem_Click(object sender, EventArgs e) =>
        new GameFileController(this).UserSave(Filename, GameName, boardControl1, lblStatus);

    private void btnSave_Click(object sender, EventArgs e) =>
        saveToolStripMenuItem_Click(sender, e);

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) =>
        new GameFileController(this).UserSaveAs(Filename, GameName, boardControl1, lblStatus);

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) =>
        Close();

    private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.UserClosing)
            return;

        if (MessageBox.Show(this, @"Are you sure you want to exit? Any unsaved changes will be lost.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            e.Cancel = true;
    }

    private void listView1_Enter(object sender, EventArgs e) =>
        boardControl1.Focus();

    private void boardControl1_Paint(object sender, PaintEventArgs e)
    {
        listView1.BeginUpdate();

        SelectInMoveList(CurrentMove + 1);

        listView1.EndUpdate();
    }

    private void RenderMoveList()
    {
        listView1.Items.Clear();

        if (Moves.Count > 0)
            listView1.Items.Add("", "START", 0);

        var moveNumber = 0;

        foreach (var move in Moves)
        {
            moveNumber++;

            var item = new ListViewItem($"{moveNumber}. {move}")
            {
                ImageIndex = move.Color == PlayerColor.White ? 1 : 2
            };

            if (move.GameEnd == EndingType.Draw)
                item.ImageIndex = 0;

            listView1.Items.Add(item);
        }

        SelectInMoveList(Moves.Count);
    }

    private void SelectNoneInMoveList()
    {
        foreach (ListViewItem item in listView1.Items)
            item.Font = listView1.Font;
    }

    private void SelectInMoveList(int index)
    {
        SelectNoneInMoveList();

        if (index >= 0 && index < listView1.Items.Count)
        {
            listView1.Items[index].Font = _boldMoveListFont;
            listView1.Items[index].EnsureVisible();
        }
    }

    private void boardControl1_PieceSelected(object sender, PieceSelectedEventArgs e)
    {
        boardControl1.CalculateCoverage();
        boardControl1.Invalidate();
        UpdateSelectedPieceProperties();
    }

    private void UpdateSelectedPieceProperties()
    {
        lvProperties.Items.Clear();

        if (boardControl1.TryGetSelectedPiece(out var point, out var piece))
            UpdatePieceProperties(point, piece);
        else
            ViewGraveyard();
    }

    private void ViewGraveyard()
    {
        if (lvProperties.Items.Count > 0)
            lvProperties.Items.Clear();

        var item = lvProperties.Items.Add("Graveyard:");
        item.Font = new Font(lvProperties.Font, FontStyle.Bold);
        item.ImageIndex = 0;
        lvProperties.Items.Add("");

        if (_graveyard.Count > 0)
        {
            foreach (var piece in _graveyard)
            {
                var pieceItem = lvProperties.Items.Add(piece.Type.ToString());
                pieceItem.ImageIndex = piece.Color == PlayerColor.White ? 1 : 2;
            }
        }
        else
        {
            lvProperties.Items.Add("Empty");
        }
    }

    private void UpdatePieceProperties(Point point, Piece piece)
    {
        var file = (char)('A' + point.X);
        var rank = point.Y + 1;
        lvProperties.Items.Add("Piece ID:", 0);
        lvProperties.Items.Add(piece.PieceId.ToString());
        lvProperties.Items.Add("");
        lvProperties.Items.Add("Piece type:", 0);
        lvProperties.Items.Add(piece.Type.ToString());
        lvProperties.Items.Add("");
        lvProperties.Items.Add("Position:", 0);
        lvProperties.Items.Add($"{file}{rank}");
        lvProperties.Items.Add("");
        lvProperties.Items.Add("Move count:", 0);
        lvProperties.Items.Add(piece.MoveCount.ToString());
    }

    private void showWhiteCoverageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        showWhiteCoverageToolStripMenuItem.Checked = !showWhiteCoverageToolStripMenuItem.Checked;
        boardControl1.ShowWhiteCoverage = showWhiteCoverageToolStripMenuItem.Checked;
        boardControl1.CalculateCoverage();
        boardControl1.Invalidate();
    }

    private void showBlackCoverageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        showBlackCoverageToolStripMenuItem.Checked = !showBlackCoverageToolStripMenuItem.Checked;
        boardControl1.ShowBlackCoverage = showBlackCoverageToolStripMenuItem.Checked;
        boardControl1.CalculateCoverage();
        boardControl1.Invalidate();
    }

    private void showSelectedPieceCoverageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        showSelectedPieceCoverageToolStripMenuItem.Checked = !showSelectedPieceCoverageToolStripMenuItem.Checked;
        boardControl1.ShowSelectedPieceCoverage = showSelectedPieceCoverageToolStripMenuItem.Checked;
        boardControl1.CalculateCoverage();
        boardControl1.Invalidate();
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Home)
            firstToolStripMenuItem_Click(sender, e);
        else if (e.KeyCode == Keys.End)
            lastToolStripMenuItem_Click(sender, e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Left)
            previousToolStripMenuItem_Click(this, EventArgs.Empty);
        else if (keyData == Keys.Right)
            nextToolStripMenuItem_Click(this, EventArgs.Empty);

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void moveToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
    {
        UpdateControls();
    }

    private void deleteLastMoveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Moves.Count <= 0)
            return;

        var oldCurrentMove = CurrentMove;
        lastToolStripMenuItem_Click(sender, e);

        if (MessageBox.Show(this, @"Are you sure you want to delete the last move?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            firstToolStripMenuItem_Click(sender, e);

            if (oldCurrentMove >= 0)
            {
                for (var i = 0; i <= oldCurrentMove; i++)
                    nextToolStripMenuItem_Click(sender, e);
            }

            return;
        }

        _registerMoveMode = false;
        boardControl1.CancelMoveRegistration();
        Moves.RemoveAt(Moves.Count - 1);
        RenderMoveList();
        GoToMove(Moves.Count - 1);
    }

    private void fromWhitesPerspectiveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var fromWhite = !ViewFromBlacksPerspective;
        fromWhitesPerspectiveToolStripMenuItem.Checked = true;
        fromBlacksPerspectiveToolStripMenuItem.Checked = false;

        if (fromWhite != !ViewFromBlacksPerspective)
            boardControl1.SetPerspective(ViewFromBlacksPerspective, _archonView);
    }

    private void fromBlacksPerspectiveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var fromBlack = ViewFromBlacksPerspective;
        fromWhitesPerspectiveToolStripMenuItem.Checked = false;
        fromBlacksPerspectiveToolStripMenuItem.Checked = true;

        if (fromBlack != ViewFromBlacksPerspective)
            boardControl1.SetPerspective(ViewFromBlacksPerspective, _archonView);
    }

    private void archonViewToolStripMenuItem_Click(object sender, EventArgs e)
    {
        archonViewToolStripMenuItem.Checked = !archonViewToolStripMenuItem.Checked;
        _archonView = archonViewToolStripMenuItem.Checked;
        boardControl1.SetPerspective(ViewFromBlacksPerspective, _archonView);
    }

    private void portableGameNotationPGNToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(this, @"Are you sure you want to import a game? Any unsaved changes will be lost.", @"Import PGN", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        using var dialog = new ImportPgnDialog();

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var contents = dialog.Contents;
            var parser = new GameParser(contents);
            var result = parser.Parse();

            if (result.Success)
            {
                _playbackTimer.Enabled = false;
                _registerMoveMode = false;
                GameName = result.GameName;
                boardControl1.GameDate = result.GameDate;
                boardControl1.WhitePlayerName = result.WhitePlayerName;
                boardControl1.BlackPlayerName = result.BlackPlayerName;
                Moves = result.Moves;
                Filename = "";
                RenderMoveList();
                GoToMove(-1);
                var message = result.Message.Trim();

                if (!string.IsNullOrWhiteSpace(message))
                    MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (listView1.Items.Count > 0)
                    listView1.Items[0].EnsureVisible();
            }
            else
            {
                MessageBox.Show(this, result.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblStatus.Text = $@"Read PGN from clipboard ""{GameName}"" ({contents.Length} characters).";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            MessageBox.Show(this, $@"The game could not be opened.{Environment.NewLine}{Environment.NewLine}{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void forsythEdwardsNotationFENToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(this, @"Are you sure you want to import a game? Any unsaved changes will be lost.", @"Import FEN", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        using var dialog = new ImportFenDialog();

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var contents = dialog.Contents;
            var parser = new GameParser(contents);
            var result = parser.Parse();

            if (result.Success)
            {
                _playbackTimer.Enabled = false;
                _registerMoveMode = false;
                GameName = result.GameName;
                boardControl1.GameDate = result.GameDate;
                boardControl1.WhitePlayerName = result.WhitePlayerName;
                boardControl1.BlackPlayerName = result.BlackPlayerName;
                Moves = result.Moves;
                Filename = "";
                RenderMoveList();
                GoToMove(-1);
                var message = result.Message.Trim();

                if (!string.IsNullOrWhiteSpace(message))
                    MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (listView1.Items.Count > 0)
                    listView1.Items[0].EnsureVisible();
            }
            else
            {
                MessageBox.Show(this, result.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblStatus.Text = $@"Read FEN from clipboard ""{GameName}"" ({contents.Length} characters).";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            MessageBox.Show(this, $@"The game could not be opened.{Environment.NewLine}{Environment.NewLine}{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnRotateBoard_Click(object sender, EventArgs e)
    {
        var w = fromWhitesPerspectiveToolStripMenuItem.Checked;
        var b = fromBlacksPerspectiveToolStripMenuItem.Checked;
        var a = archonViewToolStripMenuItem.Checked;

        if (w && !b && !a)
        {
            archonViewToolStripMenuItem_Click(sender, e);
            return;
        }

        if (w && !b && a)
        {
            archonViewToolStripMenuItem_Click(sender, e);
            fromBlacksPerspectiveToolStripMenuItem_Click(sender, e);
            return;
        }

        if (!w && b && !a)
        {
            archonViewToolStripMenuItem_Click(sender, e);
            return;
        }

        if (!w && b && a)
        {
            archonViewToolStripMenuItem_Click(sender, e);
            fromWhitesPerspectiveToolStripMenuItem_Click(sender, e);
        }
    }

    private void openTheManualToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(this, @"Open the manual at https://ahesselbom.se/chess/manual.html?", @"Open the manual", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            Process.Start("https://ahesselbom.se/chess/manual.html");
    }

    private void MainWindow_Load(object sender, EventArgs e)
    {
        ViewGraveyard();
        GameName = "";
    }

    private void registerGameEndingToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Moves.IsGameEnded || _registerMoveMode || _playbackTimer.Enabled)
            return;

        using var dialog = new RegisterGameEndDialog { MoveNumber = Moves.Count };
        
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var move = dialog.ChessMove;

        if (move == null)
            throw new SystemException("Things are not good.");

        Moves.Add(move);
        RenderMoveList();
        lastToolStripMenuItem_Click(sender, e);
    }

    private void gamePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var x = new GamePropertiesDialog();
        x.GameTitle = (GameName ?? "").Trim();
        x.GameDate = boardControl1.GameDate;
        x.WhitePlayerName = boardControl1.WhitePlayerName;
        x.BlackPlayerName = boardControl1.BlackPlayerName;
        x.Moves = Moves;

        if (x.ShowDialog(this) != DialogResult.OK)
            return;

        boardControl1.GameDate = x.GameDate;
        boardControl1.WhitePlayerName = x.WhitePlayerName;
        boardControl1.BlackPlayerName = x.BlackPlayerName;
        GameName = x.GameTitle;
        boardControl1.Invalidate();
    }

    private void registerComputerMoveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var legalMoves = new ChessBrain(boardControl1.GetCurrentBoardData(), Moves).GetLegalMoves();

        if (legalMoves.Count <= 0)
        {
            MessageBox.Show(this, @"No legal moves available now.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var moveIndex = Moves.Count;
        Move? move;

        if (legalMoves.Count == 1)
        {
            move = legalMoves[0];
        }
        else
        {
            switch (_computerLevelSkill)
            {
                case ComputerPlayerSkill.Impossible:
                    move = ComputerChessMovePicker.GetImpossibleMove(Moves.Count, legalMoves);
                    break;
                case ComputerPlayerSkill.Brutal:
                    move = ComputerChessMovePicker.GetBrutalMove(Moves.Count, legalMoves);
                    break;
                case ComputerPlayerSkill.Challenging:
                    move = ComputerChessMovePicker.GetChallengingMove(Moves.Count, legalMoves);
                    break;
                case ComputerPlayerSkill.Moderate:
                    move = ComputerChessMovePicker.GetModerateMove(legalMoves);
                    break;
                case ComputerPlayerSkill.Casual:
                    move = ComputerChessMovePicker.GetCasualMove(legalMoves);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        var piece = move.Piece!.Value;
        Moves.Add(new Move(move.StartPoint, move.EndPoint, piece, move.Color, move.PieceId!.Value, moveIndex, move.Promotion));
        RenderMoveList();

        try
        {
            GoToMove(moveIndex);
        }
        catch (Exception exception)
        {
            listView1.Items.RemoveAt(listView1.Items.Count - 1);
            Moves.RemoveAt(Moves.Count - 1);
            lastToolStripMenuItem_Click(sender, e);
            MessageBox.Show(this, exception.Message, @"This move cannot be stored", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnRegisterComputerMove_Click(object sender, EventArgs e) =>
        registerComputerMoveToolStripMenuItem_Click(sender, e);

    private void impossibleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _computerLevelSkill = ComputerPlayerSkill.Impossible;
        impossibleToolStripMenuItem.Checked = true;
        brutalToolStripMenuItem.Checked = false;
        challengingToolStripMenuItem.Checked = false;
        moderateToolStripMenuItem.Checked = false;
        casualToolStripMenuItem.Checked = false;
    }

    private void brutalToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _computerLevelSkill = ComputerPlayerSkill.Brutal;
        impossibleToolStripMenuItem.Checked = false;
        brutalToolStripMenuItem.Checked = true;
        challengingToolStripMenuItem.Checked = false;
        moderateToolStripMenuItem.Checked = false;
        casualToolStripMenuItem.Checked = false;
    }

    private void challengingToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _computerLevelSkill = ComputerPlayerSkill.Challenging;
        impossibleToolStripMenuItem.Checked = false;
        brutalToolStripMenuItem.Checked = false;
        challengingToolStripMenuItem.Checked = true;
        moderateToolStripMenuItem.Checked = false;
        casualToolStripMenuItem.Checked = false;
    }

    private void moderateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _computerLevelSkill = ComputerPlayerSkill.Moderate;
        impossibleToolStripMenuItem.Checked = false;
        brutalToolStripMenuItem.Checked = false;
        challengingToolStripMenuItem.Checked = false;
        moderateToolStripMenuItem.Checked = true;
        casualToolStripMenuItem.Checked = false;
    }

    private void casualToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _computerLevelSkill = ComputerPlayerSkill.Casual;
        impossibleToolStripMenuItem.Checked = false;
        brutalToolStripMenuItem.Checked = false;
        challengingToolStripMenuItem.Checked = false;
        moderateToolStripMenuItem.Checked = false;
        casualToolStripMenuItem.Checked = true;
    }

    private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        var item = listView1.GetItemAt(e.X, e.Y);

        if (item == null)
            return;

        var index = listView1.Items.IndexOf(item);
    }

    private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
    {
        configureBoardToolStripMenuItem.Enabled = Moves.Count <= 0;
    }

    private void configureBoardToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }
}
