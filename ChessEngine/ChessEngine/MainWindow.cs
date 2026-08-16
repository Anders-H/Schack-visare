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
    }

    public int CurrentMove
    {
        get;
        set
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
        _registerMoveMode = true;
        boardControl1.BeginMoveRegistration();
        registerMoveToolStripMenuItem.Enabled = false;
        cancelRegisterMoveToolStripMenuItem.Enabled = true;
        UpdateStatus();
    }

    private void cancelRegisterMoveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _registerMoveMode = false;
        boardControl1.CancelMoveRegistration();
        registerMoveToolStripMenuItem.Enabled = true;
        cancelRegisterMoveToolStripMenuItem.Enabled = false;
        UpdateStatus();
    }

    private void boardControl1_MoveSelected(object sender, MoveSelectedEventArgs e)
    {
        if (!_registerMoveMode)
            return;

        var movedPiece = boardControl1.GetPieceAt(e.StartPoint.X, e.StartPoint.Y);

        if (!movedPiece.HasValue)
            throw new InvalidOperationException($@"No piece at {e.StartPoint}.");

        var updatedMovedPiece = movedPiece.Value;
        updatedMovedPiece.IncreaseMoveCount();
        var moveIndex = Moves.Count;
        var capturedPiece = boardControl1.GetPieceAt(e.EndPoint.X, e.EndPoint.Y);

        if (capturedPiece.HasValue)
        {
            var updatedCapturedPiece = capturedPiece.Value;
            updatedCapturedPiece.SetDiedAtMove(moveIndex);

            boardControl1.AddToListOfBeatenPieces(
                updatedCapturedPiece,
                e.EndPoint.X,
                e.EndPoint.Y);
        }

        boardControl1.ClearSquare(e.StartPoint.X, e.StartPoint.Y);
        boardControl1.SetPieceAt(updatedMovedPiece, e.EndPoint.X, e.EndPoint.Y);

        Moves.Add(new Move(
            e.StartPoint,
            e.EndPoint,
            updatedMovedPiece.Type,
            updatedMovedPiece.Color,
            updatedMovedPiece.PieceId,
            moveIndex));

        _registerMoveMode = false;
        CurrentMove = moveIndex;
        boardControl1.CancelMoveRegistration();
        registerMoveToolStripMenuItem.Enabled = true;
        cancelRegisterMoveToolStripMenuItem.Enabled = false;
    }
}
