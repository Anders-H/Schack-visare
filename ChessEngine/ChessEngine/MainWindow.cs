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
        var y = ClientRectangle.Y + menuStrip1.Height;
        var height = ClientRectangle.Height - (menuStrip1.Height + statusStrip1.Height);
        var boardSize = Math.Max(0, Math.Min(ClientRectangle.Width, height));
        var x = ClientRectangle.X + (ClientRectangle.Width - boardSize) / 2;
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

        _registerMoveMode = false;
        e.Piece.IncreaseMoveCount();
        var deadPiece = boardControl1.GetPieceAt(e.EndPoint.X, e.EndPoint.Y);
        
        if (deadPiece.HasValue)
        {
            deadPiece.Value.SetDiedAtMove(Moves.Count + 1);
            boardControl1.AddToListOfBeatenPieces(deadPiece.Value, e.EndPoint.X, e.EndPoint.Y);
        }

        var move = new Move(e.StartPoint, e.EndPoint, e.Piece.Type, e.Piece.Color, e.Piece.PieceId, Moves.Count);
        Moves.Add(move);
        CurrentMove = Moves.Count - 1;
        var movedPiece = boardControl1.GetPieceAt(e.StartPoint.X, e.StartPoint.Y);

        if (!movedPiece.HasValue)
            throw new InvalidOperationException($@"No piece at {e.StartPoint}.");

        boardControl1.ClearSquare(e.StartPoint.X, e.StartPoint.Y);
        boardControl1.SetPieceAt(movedPiece.Value, e.EndPoint.X, e.EndPoint.Y);
        _registerMoveMode = false;
        boardControl1.Cursor = Cursors.Default;
        registerMoveToolStripMenuItem.Enabled = true;
        cancelRegisterMoveToolStripMenuItem.Enabled = false;
        boardControl1.Invalidate();
        UpdateStatus();
    }
}
