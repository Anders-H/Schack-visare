#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ChessEngine.Dialogs.DialogControls;
using ChessEngine.Events;
using ChessEngine.Moves;
using ChessEngine.Pieces;

namespace ChessEngine;

public sealed partial class BoardControl : UserControl
{
    private readonly List<Point> _whiteCoverage = [];
    private readonly List<Point> _blackCoverage = [];
    private readonly List<Point> _selectedPieceCoverage = [];
    private readonly List<Point> _validMoves = [];
    private const int BoardLength = 8;
    private const int SpriteColumns = 6;
    private const int SpriteRows = 2;
    private const float ShadowOpacity = 0.2f;
    public DateTime GameDate { get; set; }
    public string WhitePlayerName { get; set; }
    public string BlackPlayerName { get; set; }

    private static readonly Bitmap PieceSprites = Properties.Resources.pieces;
    private static readonly Bitmap PieceShadow = Properties.Resources.Shadow;
    private static readonly Color LightSquareColor = Color.FromArgb(240, 217, 181);
    private static readonly Color DarkSquareColor = Color.FromArgb(181, 136, 99);
    private static readonly Point[] OrthogonalDirections = [new(0, 1), new(1, 0), new(0, -1), new(-1, 0)];
    private static readonly Point[] DiagonalDirections = [new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)];
    private static readonly Point[] KnightOffsets = [new(1, 2), new(2, 1), new(2, -1), new(1, -2), new(-1, -2), new(-2, -1), new(-2, 1), new(-1, 2)];

    private BoardData _boardData = new();
    private bool _registerMoveMode;
    private bool _viewFromBlackPerspective;
    private bool _archonView;
    private Point? _selectedSquare;
    private Move? _selectedMove;
    private Point? _mouseDownSquare;
    private Point _mouseDownLocation;
    private Point? _dragStart;
    private Piece? _dragPiece;
    private bool _suppressClick;

    public Piece? SelectedPiece { get; set; }
    public event EventHandler<MoveSelectedEventArgs>? MoveSelected;
    public event EventHandler<PieceSelectedEventArgs>? PieceSelected;
    public event EventHandler<CancelEventArgs>? MoveDragStarted;
    public event EventHandler? MoveDragCancelled;

    public bool ShowWhiteCoverage { get; set; }
    public bool ShowBlackCoverage { get; set; }
    public bool ShowSelectedPieceCoverage { get; set; }

    public BoardControl()
    {
        SelectedPiece = null;
        GameDate = DateTime.Now;
        WhitePlayerName = "White";
        BlackPlayerName = "Black";
        InitializeComponent();
        AllowDrop = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
    }

    public BoardData GetCurrentBoardData() =>
        _boardData;

    public PieceAtSquare[,] GetBasicBoardData()
    {
        var boardData = new PieceAtSquare[BoardLength, BoardLength];

        for (var row = 0; row < BoardLength; row++)
        {
            for (var column = 0; column < BoardLength; column++)
            {
                boardData[row, column] = GetBasicBoardDataAt(row, column);
            }
        }

        return boardData;
    }

    private PieceAtSquare GetBasicBoardDataAt(int row, int col)
    {
        if (!_boardData[row, col].HasValue)
            return PieceAtSquare.None;

        var pieceType = _boardData[row, col]!.Value.Type;
        var color = _boardData[row, col]!.Value.Color;

        switch (color)
        {
            case PlayerColor.White:
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return PieceAtSquare.WhitePawn;
                    case PieceType.Rook:
                        return PieceAtSquare.WhiteRook;
                    case PieceType.Knight:
                        return PieceAtSquare.WhiteKnight;
                    case PieceType.Bishop:
                        return PieceAtSquare.WhiteBishop;
                    case PieceType.Queen:
                        return PieceAtSquare.WhiteQueen;
                    case PieceType.King:
                        return PieceAtSquare.WhiteKing;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            case PlayerColor.Black:
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return PieceAtSquare.BlackPawn;
                    case PieceType.Rook:
                        return PieceAtSquare.BlackRook;
                    case PieceType.Knight:
                        return PieceAtSquare.BlackKnight;
                    case PieceType.Bishop:
                        return PieceAtSquare.BlackBishop;
                    case PieceType.Queen:
                        return PieceAtSquare.BlackQueen;
                    case PieceType.King:
                        return PieceAtSquare.BlackKing;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Piece? GetPieceAt(int x, int y) =>
        _boardData[y, x];

    public bool TryGetSelectedPiece(out Point point, out Piece piece)
    {
        if (SelectedPiece.HasValue)
        {
            for (var row = 0; row < BoardLength; row++)
            {
                for (var column = 0; column < BoardLength; column++)
                {
                    var candidate = _boardData[row, column];

                    if (!candidate.HasValue || candidate.Value.PieceId != SelectedPiece.Value.PieceId)
                        continue;

                    point = new Point(column, row);
                    piece = candidate.Value;
                    return true;
                }
            }
        }

        point = Point.Empty;
        piece = default;
        return false;
    }

    public void SetPosition(BoardData boardData)
    {
        _boardData = boardData ?? throw new ArgumentNullException(nameof(boardData));
        _selectedSquare = null;
        _validMoves.Clear();
        Invalidate();
    }

    public void SetSelectedMove(Move? move)
    {
        _selectedMove = move?.GameEnd == EndingType.MoveIsNotGameEnd ? move : null;
        Invalidate();
    }

    public void BeginMoveRegistration()
    {
        _registerMoveMode = true;
        var hasSelectedPiece = TryGetSelectedPiece(out var point, out var piece);
        SelectedPiece = null;
        _selectedSquare = null;
        _validMoves.Clear();
        Cursor = Cursors.Cross;

        if (hasSelectedPiece)
            SelectMoveStart(point, piece);

        Invalidate();
    }

    public void CancelMoveRegistration()
    {
        _registerMoveMode = false;
        _selectedSquare = null;
        _validMoves.Clear();
        Cursor = Cursors.Default;
        Invalidate();
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
        var markerPadding = Math.Max(1f, Math.Min(4f, squareSize * 0.08f));
        var markerDiameter = Math.Max(2f, Math.Min(16f, squareSize * 0.18f));
        using var lightSquareBrush = new SolidBrush(LightSquareColor);
        using var darkSquareBrush = new SolidBrush(DarkSquareColor);
        using var selectionBrush = new SolidBrush(Color.FromArgb(110, 255, 215, 0));
        using var whiteCoverageBrush = new SolidBrush(Color.FromArgb(225, 255, 255, 255));
        using var blackCoverageBrush = new SolidBrush(Color.FromArgb(225, 0, 0, 0));
        using var selectedPieceCoverageBrush = new SolidBrush(Color.FromArgb(225, 230, 30, 30));
        using var validMoveBrush = new SolidBrush(Color.FromArgb(225, 35, 180, 70));
        using var darkMarkerPen = new Pen(Color.FromArgb(210, 35, 35, 35), 1f);
        using var lightMarkerPen = new Pen(Color.FromArgb(220, 255, 255, 255), 1f);
        using var redMarkerPen = new Pen(Color.FromArgb(220, 100, 0, 0), 1f);
        using var greenMarkerPen = new Pen(Color.FromArgb(220, 10, 100, 35), 1f);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        PointF? moveStart = null;
        PointF? moveEnd = null;
        using var shadowAttributes = new ImageAttributes();
        shadowAttributes.SetColorMatrix(new ColorMatrix { Matrix33 = ShadowOpacity });

        for (var displayRow = 0; displayRow < BoardLength; displayRow++)
        {
            for (var displayColumn = 0; displayColumn < BoardLength; displayColumn++)
            {
                var boardPoint = DisplayToBoardSquare(displayColumn, displayRow);
                var column = boardPoint.X;
                var boardRow = boardPoint.Y;
                var square = new RectangleF(
                    boardLeft + displayColumn * squareSize,
                    boardTop + displayRow * squareSize,
                    squareSize,
                    squareSize);

                var squareBrush = (boardRow + column) % 2 == 0
                    ? darkSquareBrush
                    : lightSquareBrush;

                // Use the same mapping as the pieces in every board perspective.
                if (_selectedMove != null)
                {
                    var center = new PointF(square.Left + squareSize / 2f, square.Top + squareSize / 2f);
                    if (boardPoint == _selectedMove.StartPoint)
                        moveStart = center;
                    if (boardPoint == _selectedMove.EndPoint)
                        moveEnd = center;
                }

                e.Graphics.FillRectangle(squareBrush, square);

                if (_selectedSquare == new Point(column, boardRow))
                {
                    e.Graphics.FillRectangle(selectionBrush, square);
                }

                var piece = _boardData[boardRow, column];

                if (piece.HasValue)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    
                    PointF[] shadowPoints =
                    [
                        new(square.Left, square.Top),
                        new(square.Right, square.Top),
                        new(square.Left, square.Bottom)
                    ];

                    e.Graphics.DrawImage(PieceShadow, shadowPoints, new RectangleF(0, 0, PieceShadow.Width, PieceShadow.Height), GraphicsUnit.Pixel, shadowAttributes);
                    DrawPiece(e.Graphics, piece.Value.Type, piece.Value.Color, square);
                }
                else if (_selectedMove != null && boardPoint == _selectedMove.StartPoint)
                {
                    // Loaded moves may omit the piece type; recover it from the destination.
                    var movedPieceType = _selectedMove.Piece ?? (_selectedMove.Promotion.HasValue ? PieceType.Pawn : GetPieceAt(_selectedMove.EndPoint.X, _selectedMove.EndPoint.Y)?.Type);

                    if (movedPieceType.HasValue)
                        DrawPiece(e.Graphics, movedPieceType.Value, _selectedMove.Color, square, 0.25f);
                }

                if (!_registerMoveMode && SelectedPiece.HasValue && piece.HasValue && piece.Value.PieceId == SelectedPiece.Value.PieceId)
                {
                    var width = Width switch
                    {
                        < 400 => 2,
                        > 800 => 5,
                        _ => 3
                    };

                    using var pen = new Pen(Color.FromArgb(150, 255, 0, 0), width);
                    e.Graphics.DrawRectangle(
                        pen,
                        square.Left + 1.5f,
                        square.Top + 1.5f,
                        square.Width - 3f,
                        square.Height - 3f);
                }

                if (ShowWhiteCoverage && _whiteCoverage.Contains(boardPoint))
                {
                    DrawCoverageMarker(
                        e.Graphics,
                        square.Left + markerPadding,
                        square.Top + markerPadding,
                        markerDiameter,
                        whiteCoverageBrush,
                        darkMarkerPen);
                }

                if (ShowBlackCoverage && _blackCoverage.Contains(boardPoint))
                {
                    DrawCoverageMarker(
                        e.Graphics,
                        square.Right - markerPadding - markerDiameter,
                        square.Top + markerPadding,
                        markerDiameter,
                        blackCoverageBrush,
                        lightMarkerPen);
                }

                if (ShowSelectedPieceCoverage &&
                    SelectedPiece.HasValue &&
                    _selectedPieceCoverage.Contains(boardPoint))
                {
                    DrawCoverageMarker(
                        e.Graphics,
                        square.Right - markerPadding - markerDiameter,
                        square.Bottom - markerPadding - markerDiameter,
                        markerDiameter,
                        selectedPieceCoverageBrush,
                        redMarkerPen);
                }

                if (_validMoves.Contains(boardPoint))
                {
                    DrawCoverageMarker(
                        e.Graphics,
                        square.Left + markerPadding,
                        square.Bottom - markerPadding - markerDiameter,
                        markerDiameter,
                        validMoveBrush,
                        greenMarkerPen);
                }
            }
        }

        if (moveStart.HasValue && moveEnd.HasValue)
        {
            using var movePen = new Pen(Color.FromArgb(190, 30, 100, 220), Math.Max(2f, squareSize * 0.06f));
            movePen.StartCap = LineCap.Round;
            movePen.EndCap = LineCap.Round;
            e.Graphics.DrawLine(movePen, moveStart.Value, moveEnd.Value);
        }

        const string rowNames = "ABCDEFGH";
        var textHeight = e.Graphics.MeasureString("A", Font).Height;

        for (var displayRow = 0; displayRow < BoardLength; displayRow++)
        {
            for (var displayColumn = 0; displayColumn < BoardLength; displayColumn++)
            {
                var boardPoint = DisplayToBoardSquare(displayColumn, displayRow);
                var column = boardPoint.X;
                var boardRow = boardPoint.Y;
                var squareBrush = (boardRow + column) % 2 == 0 ? lightSquareBrush : darkSquareBrush;
                var bottomLabel = _archonView ? (boardRow + 1).ToString() : rowNames[column].ToString();
                var leftLabel = _archonView ? rowNames[column].ToString() : (boardRow + 1).ToString();
                var label = displayRow == BoardLength - 1 ? bottomLabel + (displayColumn == 0 ? leftLabel : "") : displayColumn == 0 ? leftLabel : "";

                if (label.Length > 0)
                {
                    e.Graphics.DrawString(label, Font, squareBrush,
                        boardLeft + squareSize * displayColumn + 1,
                        boardTop + squareSize * displayRow + squareSize - textHeight);
                }
            }
        }

        if (_archonView)
        {
            // Game date a bit closer to the upper left corner in archon view.
            e.Graphics.DrawString($"{GameDate:yyyy-MM-dd}", Font, Brushes.Black, 2, 2);
            var leftName = _viewFromBlackPerspective ? BlackPlayerName : WhitePlayerName;
            var rightName = _viewFromBlackPerspective ? WhitePlayerName : BlackPlayerName;

            e.Graphics.DrawString(leftName, Font,
                _viewFromBlackPerspective ? Brushes.Black : Brushes.White,
                boardLeft + 2, boardTop + boardSize - 2 * textHeight - 2);
            
            e.Graphics.DrawString(rightName, Font,
                _viewFromBlackPerspective ? Brushes.White : Brushes.Black,
                boardLeft + boardSize - e.Graphics.MeasureString(rightName, Font).Width - 2,
                boardTop + boardSize - 2 * textHeight - 2);
            
            return;
        }

        // Game date on original place.
        e.Graphics.DrawString($"{GameDate:yyyy-MM-dd}", Font, Brushes.Black, textHeight + 2, textHeight + 2);
        e.Graphics.DrawString(_viewFromBlackPerspective ? WhitePlayerName : BlackPlayerName, Font, _viewFromBlackPerspective ? Brushes.White : Brushes.Black, textHeight + 2, textHeight + textHeight + 2);
        e.Graphics.DrawString(_viewFromBlackPerspective ? BlackPlayerName : WhitePlayerName, Font, _viewFromBlackPerspective ? Brushes.Black : Brushes.White, textHeight + 2, Height - (textHeight + textHeight + 2));
    }

    private static void DrawPiece(Graphics graphics, PieceType pieceType, PlayerColor color, RectangleF square, float opacity = 1f)
    {
        var spriteWidth = PieceSprites.Width / SpriteColumns - 2;
        var spriteHeight = PieceSprites.Height / SpriteRows - 2;
        var typeIndex = (int)pieceType;
        var spriteColumn = color == PlayerColor.White ? typeIndex : SpriteColumns - 1 - typeIndex;
        var spriteRow = color == PlayerColor.White ? 0 : 1;
        var source = new Rectangle(spriteColumn * spriteWidth, spriteRow * spriteHeight, spriteWidth, spriteHeight);
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
        if (opacity >= 1f)
        {
            graphics.DrawImage(PieceSprites, destination, source, GraphicsUnit.Pixel);
            return;
        }

        using var attributes = new ImageAttributes();
        attributes.SetColorMatrix(new ColorMatrix { Matrix33 = opacity });
        PointF[] destinationPoints =
        [
            new(destination.Left, destination.Top),
            new(destination.Right, destination.Top),
            new(destination.Left, destination.Bottom)
        ];
        graphics.DrawImage(PieceSprites, destinationPoints, source, GraphicsUnit.Pixel, attributes);
    }

    private static void DrawCoverageMarker(Graphics graphics, float x, float y, float diameter, Brush brush, Pen outlinePen)
    {
        graphics.FillEllipse(brush, x, y, diameter, diameter);
        graphics.DrawEllipse(outlinePen, x, y, diameter, diameter);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _suppressClick = false;
        _mouseDownSquare = null;
        if (e.Button == MouseButtons.Left && TryGetBoardSquare(e.Location, out var square) &&
            GetPieceAt(square.X, square.Y).HasValue)
        {
            _mouseDownSquare = square;
            _mouseDownLocation = e.Location;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _mouseDownSquare = null;
        base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button != MouseButtons.Left || !_mouseDownSquare.HasValue)
            return;

        var dragSize = SystemInformation.DragSize;
        var clickArea = new Rectangle(
            _mouseDownLocation.X - dragSize.Width / 2,
            _mouseDownLocation.Y - dragSize.Height / 2,
            dragSize.Width, dragSize.Height);
        if (clickArea.Contains(e.Location))
            return;

        var start = _mouseDownSquare.Value;
        _mouseDownSquare = null;
        _suppressClick = true;
        if (!BeginPieceDrag(start))
            return;

        try
        {
            var data = new DataObject();
            data.SetData(typeof(BoardControl), this);
            DoDragDrop(data, DragDropEffects.Move);
        }
        finally
        {
            // Escape and dropping outside the board also cancel registration.
            if (_dragStart.HasValue)
                CancelPieceDrag();
        }
    }

    private bool BeginPieceDrag(Point start)
    {
        var piece = GetPieceAt(start.X, start.Y);
        if (!piece.HasValue)
            return false;

        var args = new CancelEventArgs(true);
        MoveDragStarted?.Invoke(this, args);
        if (args.Cancel || !_registerMoveMode)
            return false;

        // Starting registration can navigate to the last move. Never drag a
        // different piece if the position changed underneath the mouse.
        var currentPiece = GetPieceAt(start.X, start.Y);
        if (!currentPiece.HasValue || currentPiece.Value.PieceId != piece.Value.PieceId)
        {
            MoveDragCancelled?.Invoke(this, EventArgs.Empty);
            return false;
        }

        _dragStart = start;
        _dragPiece = currentPiece;
        SelectMoveStart(start, currentPiece.Value);
        Invalidate();
        return true;
    }

    private bool IsOwnPieceDrag(DragEventArgs e) =>
        _registerMoveMode && _dragStart.HasValue && _dragPiece.HasValue &&
        ReferenceEquals(e.Data?.GetData(typeof(BoardControl)), this);

    protected override void OnDragEnter(DragEventArgs e)
    {
        base.OnDragEnter(e);
        UpdateDragEffect(e);
    }

    protected override void OnDragOver(DragEventArgs e)
    {
        base.OnDragOver(e);
        UpdateDragEffect(e);
    }

    private void UpdateDragEffect(DragEventArgs e)
    {
        e.Effect = IsOwnPieceDrag(e) &&
            TryGetBoardSquare(PointToClient(new Point(e.X, e.Y)), out _)
            ? e.AllowedEffect & DragDropEffects.Move : DragDropEffects.None;
    }

    protected override void OnDragDrop(DragEventArgs e)
    {
        base.OnDragDrop(e);
        if (!IsOwnPieceDrag(e))
            return;

        if (!TryGetBoardSquare(PointToClient(new Point(e.X, e.Y)), out var end) || end == _dragStart)
        {
            CancelPieceDrag();
            return;
        }

        var start = _dragStart!.Value;
        var piece = _dragPiece!.Value;
        _dragStart = null;
        _dragPiece = null;
        _selectedSquare = null;
        _validMoves.Clear();
        Invalidate();
        e.Effect = DragDropEffects.Move;
        MoveSelected?.Invoke(this, new MoveSelectedEventArgs(start, end, piece));
    }

    private void CancelPieceDrag()
    {
        _dragStart = null;
        _dragPiece = null;
        MoveDragCancelled?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (_suppressClick || e.Button != MouseButtons.Left || !TryGetBoardSquare(e.Location, out var clickedSquare))
            return;

        if (!_registerMoveMode)
        {
            SelectPiece(clickedSquare);
            return;
        }

        SelectMoveSquare(clickedSquare);
    }

    private void SelectPiece(Point clickedSquare)
    {
        var clickedPiece = _boardData[clickedSquare.Y, clickedSquare.X];

        if (!clickedPiece.HasValue)
            return;

        if (SelectedPiece.HasValue && SelectedPiece.Value.PieceId == clickedPiece.Value.PieceId)
        {
            SelectedPiece = null;
            PieceSelected?.Invoke(this, new PieceSelectedEventArgs(clickedSquare, null));
        }
        else
        {
            SelectedPiece = clickedPiece;
            PieceSelected?.Invoke(this, new PieceSelectedEventArgs(clickedSquare, clickedPiece.Value));
        }

        _selectedSquare = null;
        Invalidate();
    }

    private void SelectMoveSquare(Point clickedSquare)
    {
        if (!_selectedSquare.HasValue)
        {
            var clickedPiece = _boardData[clickedSquare.Y, clickedSquare.X];

            if (!clickedPiece.HasValue)
            {
                return;
            }

            SelectMoveStart(clickedSquare, clickedPiece.Value);
            Invalidate();
            return;
        }

        var startSquare = _selectedSquare.Value;

        if (clickedSquare == startSquare)
        {
            _selectedSquare = null;
            _validMoves.Clear();
            Invalidate();
            return;
        }

        var selectedPiece = _boardData[startSquare.Y, startSquare.X];

        if (!selectedPiece.HasValue)
        {
            _selectedSquare = null;
            _validMoves.Clear();
            Invalidate();
            return;
        }

        var targetPiece = _boardData[clickedSquare.Y, clickedSquare.X];

        if (targetPiece.HasValue && targetPiece.Value.Color == selectedPiece.Value.Color)
        {
            SelectMoveStart(clickedSquare, targetPiece.Value);
            Invalidate();
            return;
        }

        _selectedSquare = null;
        _validMoves.Clear();
        Invalidate();

        MoveSelected?.Invoke(this, new MoveSelectedEventArgs(startSquare, clickedSquare, selectedPiece.Value));
    }

    private void SelectMoveStart(Point square, Piece piece)
    {
        _selectedSquare = square;
        _validMoves.Clear();
        CalculatePieceValidMoves(square, piece);
    }

    private bool TryGetBoardSquare(Point location, out Point boardSquare)
    {
        var boardSize = Math.Min(ClientSize.Width, ClientSize.Height);
        var boardLeft = (ClientSize.Width - boardSize) / 2f;
        var boardTop = (ClientSize.Height - boardSize) / 2f;

        if (boardSize <= 0 || location.X < boardLeft || location.X >= boardLeft + boardSize || location.Y < boardTop || location.Y >= boardTop + boardSize)
        {
            boardSquare = Point.Empty;
            return false;
        }

        var squareSize = boardSize / (float)BoardLength;
        var column = (int)((location.X - boardLeft) / squareSize);
        var displayRow = (int)((location.Y - boardTop) / squareSize);
        boardSquare = DisplayToBoardSquare(column, displayRow);
        return true;
    }

    private void BoardControl_Resize(object sender, EventArgs e) =>
        Invalidate();

    public void CalculateCoverage()
    {
        _whiteCoverage.Clear();
        _blackCoverage.Clear();
        _selectedPieceCoverage.Clear();
        _validMoves.Clear();

        if (ShowWhiteCoverage)
            CalculateWhiteCoverage();

        if (ShowBlackCoverage)
            CalculateBlackCoverage();

        if (ShowSelectedPieceCoverage && SelectedPiece.HasValue)
            CalculateSelectedPieceCoverage();

        if (SelectedPiece.HasValue)
            CalculateValidMoves();
    }

    private void CalculateWhiteCoverage()
    {
        CalculatePlayerCoverage(PlayerColor.White, _whiteCoverage);
    }

    private void CalculateBlackCoverage()
    {
        CalculatePlayerCoverage(PlayerColor.Black, _blackCoverage);
    }

    private void CalculateSelectedPieceCoverage()
    {
        if (!SelectedPiece.HasValue)
            return;

        for (var row = 0; row < BoardLength; row++)
        {
            for (var column = 0; column < BoardLength; column++)
            {
                var piece = _boardData[row, column];

                if (!piece.HasValue || piece.Value.PieceId != SelectedPiece.Value.PieceId)
                    continue;

                CalculatePieceCoverage(new Point(column, row), piece.Value, _selectedPieceCoverage);
                return;
            }
        }
    }

    private void CalculateValidMoves()
    {
        if (!SelectedPiece.HasValue)
            return;

        for (var row = 0; row < BoardLength; row++)
        {
            for (var column = 0; column < BoardLength; column++)
            {
                var piece = _boardData[row, column];

                if (!piece.HasValue || piece.Value.PieceId != SelectedPiece.Value.PieceId)
                    continue;

                CalculatePieceValidMoves(new Point(column, row), piece.Value);
                return;
            }
        }
    }

    private void CalculatePieceValidMoves(Point origin, Piece piece)
    {
        switch (piece.Type)
        {
            case PieceType.Pawn:
                AddPawnMoves(origin, piece);
                break;

            case PieceType.Knight:
                foreach (var offset in KnightOffsets)
                    AddValidMove(origin.X + offset.X, origin.Y + offset.Y, piece.Color);
                break;

            case PieceType.Bishop:
                AddSlidingValidMoves(origin, DiagonalDirections, piece.Color);
                break;

            case PieceType.Rook:
                AddSlidingValidMoves(origin, OrthogonalDirections, piece.Color);
                break;

            case PieceType.Queen:
                AddSlidingValidMoves(origin, OrthogonalDirections, piece.Color);
                AddSlidingValidMoves(origin, DiagonalDirections, piece.Color);
                break;

            case PieceType.King:
                for (var deltaY = -1; deltaY <= 1; deltaY++)
                {
                    for (var deltaX = -1; deltaX <= 1; deltaX++)
                    {
                        if (deltaX != 0 || deltaY != 0)
                            AddValidMove(origin.X + deltaX, origin.Y + deltaY, piece.Color);
                    }
                }

                AddCastlingMoves(origin, piece);
                break;
        }
    }

    private void AddPawnMoves(Point origin, Piece piece)
    {
        var direction = piece.Color == PlayerColor.White ? 1 : -1;
        var oneStepRow = origin.Y + direction;

        if (IsBoardPoint(origin.X, oneStepRow) && !_boardData[oneStepRow, origin.X].HasValue)
        {
            AddMovePoint(origin.X, oneStepRow);

            var twoStepRow = origin.Y + 2 * direction;

            if (piece.MoveCount == 0 &&
                IsBoardPoint(origin.X, twoStepRow) &&
                !_boardData[twoStepRow, origin.X].HasValue)
            {
                AddMovePoint(origin.X, twoStepRow);
            }
        }

        AddPawnCapture(origin.X - 1, oneStepRow, piece.Color);
        AddPawnCapture(origin.X + 1, oneStepRow, piece.Color);
    }

    private void AddPawnCapture(int column, int row, PlayerColor color)
    {
        if (!IsBoardPoint(column, row))
            return;

        var target = _boardData[row, column];

        if (target.HasValue && target.Value.Color != color)
            AddMovePoint(column, row);
    }

    private void AddSlidingValidMoves(
        Point origin,
        IEnumerable<Point> directions,
        PlayerColor color)
    {
        foreach (var direction in directions)
        {
            var column = origin.X + direction.X;
            var row = origin.Y + direction.Y;

            while (IsBoardPoint(column, row))
            {
                var target = _boardData[row, column];

                if (!target.HasValue)
                {
                    AddMovePoint(column, row);
                }
                else
                {
                    if (target.Value.Color != color)
                        AddMovePoint(column, row);

                    break;
                }

                column += direction.X;
                row += direction.Y;
            }
        }
    }

    private void AddCastlingMoves(Point origin, Piece king)
    {
        if (king.MoveCount != 0 || origin.X != 4)
            return;

        AddCastlingMove(origin, king, rookColumn: 0, kingDestinationColumn: 2);
        AddCastlingMove(origin, king, rookColumn: 7, kingDestinationColumn: 6);
    }

    private void AddCastlingMove(Point origin, Piece king, int rookColumn, int kingDestinationColumn)
    {
        var rook = _boardData[origin.Y, rookColumn];

        if (!rook.HasValue || rook.Value.Type != PieceType.Rook || rook.Value.Color != king.Color || rook.Value.MoveCount != 0)
            return;

        var direction = rookColumn < origin.X ? -1 : 1;

        for (var column = origin.X + direction; column != rookColumn; column += direction)
        {
            if (_boardData[origin.Y, column].HasValue)
                return;
        }

        AddMovePoint(kingDestinationColumn, origin.Y);
    }

    private void AddValidMove(int column, int row, PlayerColor color)
    {
        if (!IsBoardPoint(column, row))
            return;

        var target = _boardData[row, column];

        if (!target.HasValue || target.Value.Color != color)
            AddMovePoint(column, row);
    }

    private void AddMovePoint(int column, int row)
    {
        var point = new Point(column, row);

        if (!_validMoves.Contains(point))
            _validMoves.Add(point);
    }

    private void CalculatePlayerCoverage(PlayerColor color, List<Point> coverage)
    {
        for (var row = 0; row < BoardLength; row++)
        {
            for (var column = 0; column < BoardLength; column++)
            {
                var piece = _boardData[row, column];

                if (piece.HasValue && piece.Value.Color == color)
                    CalculatePieceCoverage(new Point(column, row), piece.Value, coverage);
            }
        }
    }

    private void CalculatePieceCoverage(Point origin, Piece piece, List<Point> coverage)
    {
        switch (piece.Type)
        {
            case PieceType.Pawn:
                var direction = piece.Color == PlayerColor.White ? 1 : -1;
                AddCoveragePoint(coverage, origin.X - 1, origin.Y + direction);
                AddCoveragePoint(coverage, origin.X + 1, origin.Y + direction);
                break;
            case PieceType.Knight:
                foreach (var offset in KnightOffsets)
                    AddCoveragePoint(coverage, origin.X + offset.X, origin.Y + offset.Y);
                break;
            case PieceType.Bishop:
                AddSlidingCoverage(origin, DiagonalDirections, coverage);
                break;
            case PieceType.Rook:
                AddSlidingCoverage(origin, OrthogonalDirections, coverage);
                break;
            case PieceType.Queen:
                AddSlidingCoverage(origin, OrthogonalDirections, coverage);
                AddSlidingCoverage(origin, DiagonalDirections, coverage);
                break;
            case PieceType.King:
                for (var deltaY = -1; deltaY <= 1; deltaY++)
                {
                    for (var deltaX = -1; deltaX <= 1; deltaX++)
                    {
                        if (deltaX != 0 || deltaY != 0)
                            AddCoveragePoint(coverage, origin.X + deltaX, origin.Y + deltaY);
                    }
                }
                break;
        }
    }

    private void AddSlidingCoverage(Point origin, IEnumerable<Point> directions, List<Point> coverage)
    {
        foreach (var direction in directions)
        {
            var column = origin.X + direction.X;
            var row = origin.Y + direction.Y;

            while (IsBoardPoint(column, row))
            {
                AddCoveragePoint(coverage, column, row);

                if (_boardData[row, column].HasValue)
                    break;

                column += direction.X;
                row += direction.Y;
            }
        }
    }

    private static void AddCoveragePoint(List<Point> coverage, int column, int row)
    {
        if (!IsBoardPoint(column, row))
            return;

        var point = new Point(column, row);

        if (!coverage.Contains(point))
            coverage.Add(point);
    }

    private static bool IsBoardPoint(int column, int row) =>
        column is >= 0 and < BoardLength && row is >= 0 and < BoardLength;

    // State, selections and coverage use board coordinates (A1 = 0,0).
    // Only rendering and hit testing translate from the displayed grid.
    private Point DisplayToBoardSquare(int column, int row)
    {
        // A clockwise quarter turn puts White on the left; Black's perspective
        // adds a half turn. Pieces stay upright because only squares are mapped.
        if (_archonView)
        {
            return _viewFromBlackPerspective
                ? new Point(BoardLength - 1 - row, BoardLength - 1 - column)
                : new Point(row, column);
        }

        return _viewFromBlackPerspective
            ? new Point(BoardLength - 1 - column, row)
            : new Point(column, BoardLength - 1 - row);
    }

    public void SetPerspective(bool viewFromBlackPerspective, bool archonView)
    {
        if (_viewFromBlackPerspective == viewFromBlackPerspective && _archonView == archonView)
            return;

        _viewFromBlackPerspective = viewFromBlackPerspective;
        _archonView = archonView;
        Invalidate();
    }
}
