#nullable enable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChessEngine;

public partial class BoardControl : UserControl
{
    private const int BoardLength = 8;
    private const int SpriteColumns = 6;
    private const int SpriteRows = 2;

    private static readonly Bitmap PieceSprites = Properties.Resources.pieces;
    private static readonly Color LightSquareColor = Color.FromArgb(240, 217, 181);
    private static readonly Color DarkSquareColor = Color.FromArgb(181, 136, 99);

    private readonly BoardData _boardData = new();
    private bool _registerMoveMode;
    private Point? _selectedSquare;

    public event EventHandler<MoveSelectedEventArgs>? MoveSelected;

    public BoardControl()
    {
        InitializeComponent();

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw,
            true);
    }

    public Piece? GetPieceAt(int x, int y) =>
        _boardData[y, x];

    public void BeginMoveRegistration()
    {
        _registerMoveMode = true;
        _selectedSquare = null;
        Cursor = Cursors.Cross;
        Invalidate();
    }

    public void CancelMoveRegistration()
    {
        _registerMoveMode = false;
        _selectedSquare = null;
        Cursor = Cursors.Default;
        Invalidate();
    }

    public void ClearSquare(int x, int y) =>
        _boardData.ClearSquare(x, y);

    public void SetPieceAt(Piece piece, int x, int y) =>
        _boardData.SetPieceAt(piece, x, y);

    public void AddToListOfBeatenPieces(Piece piece, int x, int y)
    {
        _boardData.ClearSquare(x, y);
        _boardData.AddToListOfBeatenPieces(piece);
    }

    private void BoardControl_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.Clear(BackColor);
        var boardSize = Math.Min(ClientSize.Width, ClientSize.Height);

        if (boardSize <= 0)
            return;

        var boardLeft = (ClientSize.Width - boardSize) / 2f;
        var boardTop = (ClientSize.Height - boardSize) / 2f;
        var squareSize = boardSize / (float)BoardLength;
        using var lightSquareBrush = new SolidBrush(LightSquareColor);
        using var darkSquareBrush = new SolidBrush(DarkSquareColor);
        using var selectionBrush = new SolidBrush(Color.FromArgb(110, 255, 215, 0));
        using var selectionPen = new Pen(Color.Gold, 3f);

        for (var displayRow = 0; displayRow < BoardLength; displayRow++)
        {
            var boardRow = BoardLength - 1 - displayRow;

            for (var column = 0; column < BoardLength; column++)
            {
                var square = new RectangleF(
                    boardLeft + column * squareSize,
                    boardTop + displayRow * squareSize,
                    squareSize,
                    squareSize);

                var squareBrush = (boardRow + column) % 2 == 0
                    ? darkSquareBrush
                    : lightSquareBrush;

                e.Graphics.FillRectangle(squareBrush, square);

                if (_selectedSquare == new Point(column, boardRow))
                {
                    e.Graphics.FillRectangle(selectionBrush, square);

                    e.Graphics.DrawRectangle(
                        selectionPen,
                        square.Left + 1.5f,
                        square.Top + 1.5f,
                        square.Width - 3f,
                        square.Height - 3f);
                }

                var piece = _boardData[boardRow, column];

                if (piece.HasValue)
                    DrawPiece(e.Graphics, piece.Value.Type, piece.Value.Color, square);
            }
        }

        const string rowNames = "ABCDEFGH";
        var textHeight = e.Graphics.MeasureString("A", Font).Height;

        for (var displayRow = 0; displayRow < BoardLength; displayRow++)
        {
            var boardRow = BoardLength - 1 - displayRow;

            for (var column = 0; column < BoardLength; column++)
            {
                var squareBrush = (boardRow + column) % 2 == 0
                    ? lightSquareBrush
                    : darkSquareBrush;

                if (displayRow == 7 && column == 0)
                    e.Graphics.DrawString($"{rowNames[column]}{boardRow + 1}", Font, squareBrush, squareSize * column + 1, squareSize * displayRow + squareSize - textHeight);
                else if (displayRow == 7)
                    e.Graphics.DrawString($"{rowNames[column]}", Font, squareBrush, squareSize * column + 1, squareSize * displayRow + squareSize - textHeight);
                else if (column == 0)
                    e.Graphics.DrawString($"{boardRow + 1}", Font, squareBrush, squareSize * column + 1, squareSize * displayRow + squareSize - textHeight);
            }
        }
    }

    private static void DrawPiece(Graphics graphics, PieceType pieceType, PlayerColor color, RectangleF square)
    {
        var spriteWidth = PieceSprites.Width / SpriteColumns - 2;
        var spriteHeight = PieceSprites.Height / SpriteRows - 2;
        var typeIndex = (int)pieceType;

        var spriteColumn = color == PlayerColor.White
            ? typeIndex
            : SpriteColumns - 1 - typeIndex;

        var spriteRow = color == PlayerColor.White ? 0 : 1;

        var source = new Rectangle(
            spriteColumn * spriteWidth,
            spriteRow * spriteHeight,
            spriteWidth,
            spriteHeight);

        const float pieceScale = 0.94f;
        var maximumWidth = square.Width * pieceScale;
        var maximumHeight = square.Height * pieceScale;
        var scale = Math.Min(maximumWidth / source.Width, maximumHeight / source.Height);
        var width = source.Width * scale;
        var height = source.Height * scale;

        var destination = new RectangleF(
            square.Left + (square.Width - width) / 2f,
            square.Top + (square.Height - height) / 2f,
            width,
            height);

        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.DrawImage(PieceSprites, destination, source, GraphicsUnit.Pixel);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (!_registerMoveMode || e.Button != MouseButtons.Left || !TryGetBoardSquare(e.Location, out var clickedSquare))
        {
            return;
        }

        if (!_selectedSquare.HasValue)
        {
            if (!_boardData[clickedSquare.Y, clickedSquare.X].HasValue)
            {
                return;
            }

            _selectedSquare = clickedSquare;
            Invalidate();
            return;
        }

        var startSquare = _selectedSquare.Value;

        if (clickedSquare == startSquare)
        {
            _selectedSquare = null;
            Invalidate();
            return;
        }

        var selectedPiece = _boardData[startSquare.Y, startSquare.X];
        
        if (!selectedPiece.HasValue)
        {
            _selectedSquare = null;
            Invalidate();
            return;
        }

        var targetPiece = _boardData[clickedSquare.Y, clickedSquare.X];

        if (targetPiece.HasValue && targetPiece.Value.Color == selectedPiece.Value.Color)
        {
            _selectedSquare = clickedSquare;
            Invalidate();
            return;
        }

        _selectedSquare = null;
        Invalidate();

        MoveSelected?.Invoke(this, new MoveSelectedEventArgs(startSquare, clickedSquare, selectedPiece.Value));
    }

    private bool TryGetBoardSquare(Point location, out Point boardSquare)
    {
        var boardSize = Math.Min(ClientSize.Width, ClientSize.Height);
        var boardLeft = (ClientSize.Width - boardSize) / 2f;
        var boardTop = (ClientSize.Height - boardSize) / 2f;

        if (boardSize <= 0 ||
            location.X < boardLeft ||
            location.X >= boardLeft + boardSize ||
            location.Y < boardTop ||
            location.Y >= boardTop + boardSize)
        {
            boardSquare = Point.Empty;
            return false;
        }

        var squareSize = boardSize / (float)BoardLength;
        var column = (int)((location.X - boardLeft) / squareSize);
        var displayRow = (int)((location.Y - boardTop) / squareSize);
        var row = BoardLength - 1 - displayRow;

        boardSquare = new Point(column, row);
        return true;
    }

    private void BoardControl_Resize(object sender, EventArgs e) =>
        Invalidate();
}
