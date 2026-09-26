#nullable enable
using System;
using System.Text;
using ChessEngine.Dialogs.DialogControls;
using ChessEngine.ExternalParsers;
using ChessEngine.Pieces;
using System.Windows.Forms;

namespace ChessEngine.Dialogs;

public partial class ConfigureBoardDialog : Form
{
    public GamePosition? InitialPosition { get; private set; }

    public ConfigureBoardDialog()
    {
        InitializeComponent();
    }

    private GamePosition GetInitialPosition()
        => new FenParser(GetPositionFen()).ParsePosition();

    private string GetPositionFen()
    {
        // FEN uses the same a8-to-h1 order as the dialog. Reuse the importer
        // so validation, piece IDs and saved positions follow the same rules.
        const string symbols = " PRNBQKprnbqk";
        var fen = new StringBuilder();
        
        for (var y = 0; y < 8; y++)
        {
            if (y > 0)
                fen.Append('/');

            var empty = 0;
            
            for (var x = 0; x < 8; x++)
            {
                var piece = (PieceAtSquare)GetComboBox(x, y).SelectedItem;
                
                if (piece == PieceAtSquare.None)
                {
                    empty++;
                    continue;
                }
                
                if (empty > 0)
                    fen.Append(empty);
                
                empty = 0;
                fen.Append(symbols[(int)piece]);
            }

            if (empty > 0)
                fen.Append(empty);
        }

        fen.Append(" w ").Append(GetCastlingRights()).Append(" - 0 1");
        return fen.ToString();
    }

    private string GetCastlingRights()
    {
        // A configured starting position treats kings and rooks on their home
        // squares as unmoved. Occupied/attacked paths are checked when playing.
        var rights = new StringBuilder();
        if (Equals(GetComboBox(4, 7).SelectedItem, PieceAtSquare.WhiteKing))
        {
            if (Equals(GetComboBox(7, 7).SelectedItem, PieceAtSquare.WhiteRook)) rights.Append('K');
            if (Equals(GetComboBox(0, 7).SelectedItem, PieceAtSquare.WhiteRook)) rights.Append('Q');
        }
        if (Equals(GetComboBox(4, 0).SelectedItem, PieceAtSquare.BlackKing))
        {
            if (Equals(GetComboBox(7, 0).SelectedItem, PieceAtSquare.BlackRook)) rights.Append('k');
            if (Equals(GetComboBox(0, 0).SelectedItem, PieceAtSquare.BlackRook)) rights.Append('q');
        }
        return rights.Length == 0 ? "-" : rights.ToString();
    }

    private GamePosition GetInitialPositionForced()
    {
        var board = new Piece?[8, 8];
        var id = 0;

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var selected = (PieceAtSquare)GetComboBox(x, y).SelectedItem;

                if (selected == PieceAtSquare.None)
                    continue;

                var type = selected switch
                {
                    PieceAtSquare.WhitePawn or PieceAtSquare.BlackPawn => PieceType.Pawn,
                    PieceAtSquare.WhiteRook or PieceAtSquare.BlackRook => PieceType.Rook,
                    PieceAtSquare.WhiteKnight or PieceAtSquare.BlackKnight => PieceType.Knight,
                    PieceAtSquare.WhiteBishop or PieceAtSquare.BlackBishop => PieceType.Bishop,
                    PieceAtSquare.WhiteQueen or PieceAtSquare.BlackQueen => PieceType.Queen,
                    PieceAtSquare.WhiteKing or PieceAtSquare.BlackKing => PieceType.King,
                    _ => throw new ArgumentOutOfRangeException(nameof(selected))
                };

                var color = selected <= PieceAtSquare.WhiteKing ? PlayerColor.White : PlayerColor.Black;

                board[7 - y, x] = new Piece(id++, type, color)
                {
                    MoveCount = type is PieceType.King or PieceType.Rook ? 1 : 0
                };
            }
        }

        foreach (var right in GetCastlingRights())
        {
            if (right == '-') continue;
            var row = char.IsUpper(right) ? 0 : 7;
            var column = char.ToUpperInvariant(right) == 'K' ? 7 : 0;
            var king = board[row, 4]!.Value;
            var rook = board[row, column]!.Value;
            king.MoveCount = rook.MoveCount = 0;
            board[row, 4] = king;
            board[row, column] = rook;
        }

        // Construct directly, without validating or parsing FEN. The text is
        // retained only because the existing save format uses GamePosition.Fen.
        return new GamePosition(board, GetPositionFen(), true, null, 0, 1);
    }

    private void ConfigureBoardDialog_Load(object sender, EventArgs e)
    {
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var c = GetComboBox(x, y);
                c.BackColor = (x + y) % 2 == 0 ? System.Drawing.Color.FromArgb(255, 240, 217) : System.Drawing.Color.FromArgb(181, 136, 99);
            }
        }
    }

    public void SetBoardData(PieceAtSquare[,] boardData)
    {
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var c = GetComboBox(x, y);
                // Board data is [rank - 1, file]; the dialog always shows a8 at top left.
                c.SelectedItem = boardData[7 - y, x];
            }
        }
    }

    private PieceSelector GetComboBox(int x, int y)
    {
        switch (y)
        {
            case 0:
                return x switch
                {
                    0 => pieceSelector1,
                    1 => pieceSelector2,
                    2 => pieceSelector3,
                    3 => pieceSelector4,
                    4 => pieceSelector5,
                    5 => pieceSelector6,
                    6 => pieceSelector7,
                    7 => pieceSelector8,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 1:
                return x switch
                {
                    0 => pieceSelector16,
                    1 => pieceSelector15,
                    2 => pieceSelector14,
                    3 => pieceSelector13,
                    4 => pieceSelector12,
                    5 => pieceSelector11,
                    6 => pieceSelector10,
                    7 => pieceSelector9,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 2:
                return x switch
                {
                    0 => pieceSelector24,
                    1 => pieceSelector23,
                    2 => pieceSelector22,
                    3 => pieceSelector21,
                    4 => pieceSelector20,
                    5 => pieceSelector19,
                    6 => pieceSelector18,
                    7 => pieceSelector17,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 3:
                return x switch
                {
                    0 => pieceSelector32,
                    1 => pieceSelector31,
                    2 => pieceSelector30,
                    3 => pieceSelector29,
                    4 => pieceSelector28,
                    5 => pieceSelector27,
                    6 => pieceSelector26,
                    7 => pieceSelector25,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 4:
                return x switch
                {
                    0 => pieceSelector64,
                    1 => pieceSelector63,
                    2 => pieceSelector62,
                    3 => pieceSelector61,
                    4 => pieceSelector60,
                    5 => pieceSelector59,
                    6 => pieceSelector58,
                    7 => pieceSelector57,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 5:
                return x switch
                {
                    0 => pieceSelector56,
                    1 => pieceSelector55,
                    2 => pieceSelector54,
                    3 => pieceSelector53,
                    4 => pieceSelector52,
                    5 => pieceSelector51,
                    6 => pieceSelector50,
                    7 => pieceSelector49,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 6:
                return x switch
                {
                    0 => pieceSelector48,
                    1 => pieceSelector47,
                    2 => pieceSelector46,
                    3 => pieceSelector45,
                    4 => pieceSelector44,
                    5 => pieceSelector43,
                    6 => pieceSelector42,
                    7 => pieceSelector41,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case 7:
                return x switch
                {
                    0 => pieceSelector40,
                    1 => pieceSelector39,
                    2 => pieceSelector38,
                    3 => pieceSelector37,
                    4 => pieceSelector36,
                    5 => pieceSelector35,
                    6 => pieceSelector34,
                    7 => pieceSelector33,
                    _ => throw new ArgumentOutOfRangeException()
                };
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        try
        {
            InitialPosition = GetInitialPosition();
        }
        catch (FormatException ex)
        {
            if (MessageBox.Show(this, $@"{ex.Message}
Do you want to continue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            InitialPosition = GetInitialPositionForced();
        }

        DialogResult = DialogResult.OK;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                var c = GetComboBox(x, y);
                c.SelectedIndex = 0;
            }
        }
    }

    private void btnStandard_Click(object sender, EventArgs e)
    {
        PieceAtSquare[] whiteBackRank =
        [
            PieceAtSquare.WhiteRook, PieceAtSquare.WhiteKnight,
            PieceAtSquare.WhiteBishop, PieceAtSquare.WhiteQueen,
            PieceAtSquare.WhiteKing, PieceAtSquare.WhiteBishop,
            PieceAtSquare.WhiteKnight, PieceAtSquare.WhiteRook
        ];
        
        PieceAtSquare[] blackBackRank =
        [
            PieceAtSquare.BlackRook, PieceAtSquare.BlackKnight,
            PieceAtSquare.BlackBishop, PieceAtSquare.BlackQueen,
            PieceAtSquare.BlackKing, PieceAtSquare.BlackBishop,
            PieceAtSquare.BlackKnight, PieceAtSquare.BlackRook
        ];
        
        var board = new PieceAtSquare[8, 8];

        for (var x = 0; x < 8; x++)
        {
            board[0, x] = whiteBackRank[x];
            board[1, x] = PieceAtSquare.WhitePawn;
            board[6, x] = PieceAtSquare.BlackPawn;
            board[7, x] = blackBackRank[x];
        }

        SetBoardData(board);
    }
}
