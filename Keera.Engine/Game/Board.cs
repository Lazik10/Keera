using Keera.Engine.Pieces;
using Keera.Engine.Types;

namespace Keera.Engine.Game;

public class Board
{
    private readonly BoardPosition[,] BoardPositions;
    private readonly List<string> MoveHistory;

    Game Game { get; set; }

    public const int MaxFile = 8;
    public const int MaxRank = 8;

    public Board(Game game)
    {
        Game = game;
        BoardPositions = new BoardPosition[MaxRank, MaxFile];
        MoveHistory = new List<string>();

        InitBoard();
    }

    private void InitBoard()
    {
        for (int rank = 0; rank < BoardPositions.GetLength(0); rank++)
        {
            for (int file = 0; file < BoardPositions.GetLength(1); file++)
            {
                BoardPositions[rank, file] = new BoardPosition(null, (rank + file) % 2 == 0 ? Color.Black : Color.White, new Position(rank, file));
            }
        }
    }

    public void LoadPosition(string position)
    {
        if (string.IsNullOrWhiteSpace(position))
        {
            return;
        }

        var piecesArray = new char[] { 'r', 'n', 'b', 'q', 'k', 'p' };

        var rank = MaxRank - 1;
        var file = 0;

        foreach (var p in position)
        {
            if (p == '/')
            {
                rank--;
                file = 0;
            }
            else if (p >= '1' && p <= '8')
            {
                var success = int.TryParse(p.ToString(), out var count);
                if (success)
                {
                    file += count;
                }
            }
            else if (piecesArray.Contains(char.ToLower(p)))
            {
                var piecePosition = new Position(rank, file);
                var pieceColor = char.IsUpper(p) ? Color.White : Color.Black;

                Piece piece = char.ToLower(p) switch
                {
                    'p' => new Pawn(piecePosition, pieceColor, this),
                    'n' => new Knight(piecePosition, pieceColor, this),
                    'b' => new Bishop(piecePosition, pieceColor, this),
                    'r' => new Rook(piecePosition, pieceColor, this),
                    'q' => new Queen(piecePosition, pieceColor, this),
                    'k' => new King(piecePosition, pieceColor, this),
                    _ => throw new ArgumentException("Unsupported piece character")
                };

                piece.OnPieceMoved += Piece_OnPieceMoved;

                BoardPositions[rank, file].SetPiece(piece);

                file++;
            }
            else if (p == ' ')
            {
                // TODO: Support other parts of FEN notation
                return;
            }
            else
            {
                throw new Exception("Unsupported character in FEN notation");
            }
        }
    }

    private void Piece_OnPieceMoved(object? sender, Move e)
    {
        if (sender is not Piece piece)
        {
            return;
        }

        // When Castling we need tu update also correct rook's position
        if (piece.Code == 'k' || piece.Code == 'K')
        {
            if (e.Type == MoveType.CastlingQ || e.Type == MoveType.CastlingK)
            {
                int rookOffset = e.Type == MoveType.CastlingQ ? -4 : 3;
                int moveOffset = e.Type == MoveType.CastlingQ ? 3 : -2;
                Piece? rook = GetPieceOnPosition(new Position(piece.Position.Rank, piece.Position.File + rookOffset));
                if (rook != null)
                {
                    BoardPositions[rook.Position.Rank, rook.Position.File + moveOffset].SetPiece(rook);
                    BoardPositions[rook.Position.Rank, rook.Position.File].SetPiece(null);
                }
            }
        }

        // Handle promoting
        if (piece is Pawn)
        {
            if (piece.Color == Color.White && e.EndPosition.Rank == 7 || piece.Color == Color.Black && e.EndPosition.Rank == 0)
            {
                piece = Pawn.Promote((Pawn)piece, e);
            }
        }

        BoardPositions[e.StartPosition.Rank, e.StartPosition.File].SetPiece(null);
        BoardPositions[e.EndPosition.Rank, e.EndPosition.File].SetPiece(piece);

        MoveHistory.Add(e.ToString());

        PrintBoard();
    }

    public Piece? GetPieceOnPosition(Position position)
    {
        return BoardPositions[position.Rank, position.File].Piece;
    }

    public void MovePiece(string? moveString)
    {
        if (string.IsNullOrWhiteSpace(moveString))
            return;

        Move move = Move.FromString(moveString, Game);
        var piece = GetPieceOnPosition(move.StartPosition);

        if (piece != null && piece.Color == Game.Turn)
        {
            piece.MoveTo(move.EndPosition);

            Game.Turn = Game.Turn == Color.White ? Color.Black : Color.White;
        }
    }

    public void PrintBoard()
    {
        Console.WriteLine("--------");

        for (int rank = BoardPositions.GetLength(0) - 1; rank >= 0; rank--)
        {
            for (int file = 0; file < BoardPositions.GetLength(1); file++)
            {
                Console.Write(GetPieceOnPosition(new Position(rank, file))?.Code ?? ' ');
            }

            Console.WriteLine();
        }
        Console.WriteLine("--------");
    }

    public void PrintMoveHistory()
    {
        Console.Write($"Move history: ");
        foreach (var move in MoveHistory)
        {
            Console.Write(move + " ");
            if (move == MoveHistory.Last())
                Console.Write("\n");
        }
    }
}
