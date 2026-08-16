#nullable enable
using System;
using System.Windows.Forms;
using System.Drawing;

namespace ChessEngine;

public partial class MainWindow : Form
{
    private bool _registerMoveMode;
    private MoveList Moves { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        boardControl1.MoveSelected += boardControl1_MoveSelected;
        _registerMoveMode = false;
        Moves = [];
        CurrentMove = -1;
        ResizeBoard();
        UpdateControls();
    }

    public int CurrentMove
    {
        get;
        private set
        {
            field = value;
            UpdateStatus();
        }
    }

    private void UpdateStatus() =>
        lblStatus.Text = _registerMoveMode ? $@"Storing move {Moves.Count + 1}." : $@"Move {CurrentMove + 1} of {Moves.Count}.";

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) =>
        MessageBox.Show(this, @"Chess Engine written by Anders Hesselbom. Application icon created by Vivek Kale.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void MainWindow_Resize(object sender, EventArgs e) =>
        ResizeBoard();

    private void ResizeBoard()
    {
        var y = ClientRectangle.Y + menuStrip1.Height + toolStrip1.Height;
        var height = ClientRectangle.Height - (menuStrip1.Height + statusStrip1.Height + toolStrip1.Height);
        var boardSize = Math.Max(0, Math.Min(ClientRectangle.Width - listView1.Width, height));
        var x = ClientRectangle.X + (ClientRectangle.Width - boardSize) / 2;
        x += listView1.Width / 2;
        var boardY = y + (height - boardSize) / 2;
        boardControl1.Bounds = new Rectangle(x, boardY, boardSize, boardSize);
    }

    private void registerMoveToolStripMenuItem_Click(object sender, EventArgs e)
    {
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
        if (!_registerMoveMode)
            return;

        var movedPiece = boardControl1.GetPieceAt(e.StartPoint.X, e.StartPoint.Y);

        if (!movedPiece.HasValue)
            throw new InvalidOperationException($@"No piece at {e.StartPoint}.");

        var moveIndex = Moves.Count;

        Moves.Add(new Move(
            e.StartPoint,
            e.EndPoint,
            movedPiece.Value.Type,
            movedPiece.Value.Color,
            movedPiece.Value.PieceId,
            moveIndex));

        _registerMoveMode = false;
        boardControl1.CancelMoveRegistration();
        GoToMove(moveIndex);
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
        GoToMove(-1);
    }

    private void previousToolStripMenuItem_Click(object sender, EventArgs e)
    {
        GoToMove(CurrentMove - 1);
    }

    private void playToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void nextToolStripMenuItem_Click(object sender, EventArgs e)
    {
        GoToMove(CurrentMove + 1);
    }

    private void lastToolStripMenuItem_Click(object sender, EventArgs e)
    {
        GoToMove(Moves.Count - 1);
    }

    private void GoToMove(int moveIndex)
    {
        var lastMoveIndex = Moves.Count - 1;
        var targetMoveIndex = Math.Max(-1, Math.Min(moveIndex, lastMoveIndex));
        var position = new BoardData();

        for (var index = 0; index <= targetMoveIndex; index++)
            position.ApplyMove(Moves[index]);

        boardControl1.SetPosition(position);
        CurrentMove = targetMoveIndex;
        UpdateControls();
    }

    private void UpdateControls()
    {
        var navigationEnabled = !_registerMoveMode;
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

        registerMoveToolStripMenuItem.Enabled = !_registerMoveMode;
        btnRegistrera.Enabled = !_registerMoveMode;
        cancelRegisterMoveToolStripMenuItem.Enabled = _registerMoveMode;
        btnAvbrytRegistrering.Enabled = _registerMoveMode;

        // Play och Pause kopplas in när timer-uppspelningen implementeras.
        playToolStripMenuItem.Enabled = false;
        btnPlay.Enabled = false;
        pauseToolStripMenuItem.Enabled = false;
        btnPause.Enabled = false;
    }
}
