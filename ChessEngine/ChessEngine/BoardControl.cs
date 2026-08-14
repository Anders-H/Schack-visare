#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChessEngine;

public partial class BoardControl : UserControl
{
    public BoardControl()
    {
        InitializeComponent();
    }

    private void BoardControl_Load(object sender, EventArgs e)
    {

    }

    private void BoardControl_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.Clear(Color.Green);
    }

    private void BoardControl_Resize(object sender, EventArgs e) =>
        Invalidate();
}