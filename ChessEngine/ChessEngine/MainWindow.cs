#nullable enable
using System;
using System.Windows.Forms;
using System.Drawing;

namespace ChessEngine;

public partial class MainWindow : Form
{
    public MainWindow()
    {
        InitializeComponent();
        ResizeBoard();
    }

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
        boardControl1.Invalidate();
    }
}
