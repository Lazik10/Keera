using Keera.Engine.Pieces;
using Keera.Engine.Types;
using System.Collections.Generic;
using System.Linq;

namespace Keera.Engine.Game;

public class Board
{
    //private readonly BoardPosition[,] BoardPositions;
    private readonly Dictionary<Position, BoardPosition> boardPositions;
    private readonly List<string> MoveHistory;

    public Game Game { get; set; }

    public const int MaxFile = 8;
    public const int MaxRank = 8;

    public Dictionary<Color, List<Position>> capturePositions;
    public Dictionary<Color, List<Move>> PossibleMoves { get; set; }

    public Board(Game game)
    {
        Game = game;
        boardPositions = new Dictionary<Position, BoardPosition>(MaxFile * MaxRank);
        MoveHistory = new List<string>();
        capturePositions = new Dictionary<Color, List<Position>>
        {
            [Color.White] = new List<Position>(),
            [Color.Black] = new List<Position>()
        };

        PossibleMoves = new Dictionary<Color, List<Move>>
        {
            [Color.White] = new List<Move>(),
            [Color.Black] = new List<Move>()
        };

        InitBoard();
    }

    private void InitBoard()
    {
        for (int rank = 0; rank < MaxRank; rank++)
        {
            for (int file = 0; file < MaxFile; file++)
            {
                var pos = new Position(rank, file);
                boardPositions.Add(pos, new BoardPosition(null, (rank + file) % 2 == 0 ? Color.Black : Color.White, pos));
            }
        }
    }

    public void CalculatePossibleMoves(Color color)
    {
        PossibleMoves[Color.Black].Clear();
        PossibleMoves[Color.White].Clear();

        foreach (var item in boardPositions)
        {
            if (item.Value.Piece is not null && item.Value.Piece.Color == color)
            {
                item.Value.Piece.Protected = false;
            }
        }

        if (GetKing(false) is King king && king.Checked)
        {
            // DONE: If opponent's king is under attack/check, let him play only with him next turn
            // TO DO:
            // 1) - or find and add a possible move with his piece to hide him from check
            // 2) - or add moves that allow him to capture piece who is threatening him
            PossibleMoves[color].AddRange(king.GetPossiblePositions());
        }
        else
        {
            foreach (var item in boardPositions)
            {
                if (item.Value.Piece is not null && item.Value.Piece.Color == color)
                {
                    PossibleMoves[color].AddRange(item.Value.Piece.GetPossiblePositions().Where(x => !(x.Type.HasFlag(MoveType.Check) && x.Piece is Pawn)));
                }
            }
        }

        Console.WriteLine($"{color} possible move positions: '{PossibleMoves[color].Count}' :");
        PossibleMoves[color].ForEach(x => Console.Write(x.ToString() +"(" + x.Type.ToString()+ ")" + " "));
        Console.WriteLine();
    }

    public void CalculateCapturePositions(Color color)
    {
        capturePositions[Color.White].Clear();
        capturePositions[Color.Black].Clear();
        foreach (var item in boardPositions)
        {
            if (item.Value.Piece is not null && item.Value.Piece.Color == color)
            {
                capturePositions[color].AddRange(item.Value.Piece.GetPossiblePositions().Where(x => !(x.Type.HasFlag(MoveType.Move) && x.Piece is Pawn)).Select(x => x.EndPosition));
            }
        }

        Console.WriteLine($"{color} capture positions '{capturePositions[color].Count}' :");
        capturePositions[color].ForEach(x => Console.Write(x.ToString() + " "));
        Console.WriteLine();
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

                boardPositions[piecePosition].SetPiece(piece);

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
            Console.WriteLine("Not piece");
            return;
        }

        // When Castling we need tu update also correct rook's position
        if (piece is King king)
        {
            king.Checked = false;

            if (e.Type == MoveType.CastlingQ || e.Type == MoveType.CastlingK)
            {
                int rookOffset = e.Type == MoveType.CastlingQ ? -4 : 3;
                int moveOffset = e.Type == MoveType.CastlingQ ? 3 : -2;
                Piece? rook = GetPieceOnPosition(new Position(piece.Position.Rank, piece.Position.File + rookOffset));
                if (rook != null)
                {
                    boardPositions[new Position(rook.Position.Rank, rook.Position.File + moveOffset)].SetPiece(rook);
                    boardPositions[new Position(rook.Position.Rank, rook.Position.File)].SetPiece(null);
                }
            }
        }

        if (piece is Pawn pawn)
        {
            // Handle promoting
            if (piece.Color == Color.White && e.EndPosition.Rank == 7 || piece.Color == Color.Black && e.EndPosition.Rank == 0)
            {
                piece = Pawn.Promote(pawn, e);
                piece.OnPieceMoved += Piece_OnPieceMoved;
            }

            // En Passant move
            if (e.Type == MoveType.EnPassant)
            {
                boardPositions[new Position(e.EndPosition.Rank + (piece.Color == Color.White ? -1 : 1), e.EndPosition.File)].SetPiece(null);
            }
        }

        // Swap pieces
        boardPositions[new Position(e.StartPosition.Rank, e.StartPosition.File)].SetPiece(null);
        boardPositions[new Position(e.EndPosition.Rank, e.EndPosition.File)].SetPiece(piece);

        // Info
        Console.WriteLine($"{e}{e.Type}");
        PrintBoard();

        // TODO: Find better solution
        var opponentsKingBoardPos = boardPositions.Single(x => x.Value.Piece is King && x.Value.Piece.Color != Game.Turn);
        var opponentsKing = opponentsKingBoardPos.Value.Piece;
        opponentsKingBoardPos.Value.SetPiece(null);

        // Re-calculate capture moves
        CalculateCapturePositions(Game.Turn);

        opponentsKingBoardPos.Value.SetPiece(opponentsKing);

        // Check if opponents king is under attack after piece moved
        CalculatePossibleMoves(Game.Turn);

        foreach (var item in PossibleMoves[Game.Turn])
        {
            if (item.EndPosition.Equals(opponentsKing.Position))
            {
                Console.WriteLine("CHECK");
                e.AddTypeFlag(MoveType.Check);
                if (opponentsKing is King _king)
                    _king.Checked = true;
            }
        }

        // Now check if it is not check mate
        if (e.Type == MoveType.Check)
        {
            var kingPositions = opponentsKingBoardPos.Value.Piece?.GetPossiblePositions().Select(x => x.EndPosition).Except(capturePositions[Game.Turn]);

            if (!kingPositions?.Any() ?? true)
            {
                e.AddTypeFlag(MoveType.Checkmate);
                e.RemoveTypeFlag(MoveType.Check);
                MoveHistory.Add(e.ToString());
                PrintMoveHistory();

                Game.Status = Game.GameStatus.EndedByWin;

                Game.EndGame(Game.Status, Color.All ^ Game.Turn, Game.Turn);
                return;
            }
        }

        // Record completed move
        MoveHistory.Add(e.ToString());
        PrintMoveHistory();
        Console.WriteLine("-------------------------------------------------------");

        // Change turn
        Game.ChangeTurn(e.Type);
    }

    public Piece? GetPieceOnPosition(Position position)
    {
        return boardPositions[new Position(position.Rank, position.File)].Piece;
    }

    public Piece GetKing(bool opponent)
    {
        var kingBoardPos = boardPositions.Single(x => x.Value.Piece is King && (opponent == true? x.Value.Piece.Color != Game.Turn : x.Value.Piece.Color == Game.Turn));
        var king = kingBoardPos.Value.Piece;
        return king;
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
        }
    }

    public void PrintBoard()
    {
        Console.WriteLine("     a   b   c   d   e   f   g   h  ");
        Console.WriteLine("    ─── ─── ─── ─── ─── ─── ─── ─── ");

        for (int rank = MaxRank - 1; rank >= 0; rank--)
        {
            Console.Write($" {rank + 1} │");
            for (int file = 0; file < MaxFile; file++)
            {
                Console.Write($" {GetPieceOnPosition(new Position(rank, file))?.Code ?? ' '} ");
                Console.Write("│");
            }
            Console.Write($" {rank + 1}");

            Console.WriteLine();
            Console.WriteLine("    ─── ─── ─── ─── ─── ─── ─── ─── ");
        }
        Console.WriteLine("     a   b   c   d   e   f   g   h  ");
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
