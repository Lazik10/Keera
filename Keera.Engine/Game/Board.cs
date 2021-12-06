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

    Game Game { get; set; }

    public const int MaxFile = 8;
    public const int MaxRank = 8;

    private Dictionary<Color, List<Position>> capturePositions;

    public Board(Game game)
    {
        Game = game;
        //BoardPositions = new BoardPosition[MaxRank, MaxFile];
        boardPositions = new Dictionary<Position, BoardPosition>(MaxFile * MaxRank);
        MoveHistory = new List<string>();
        capturePositions = new Dictionary<Color, List<Position>>();
        capturePositions[Color.White] = new List<Position>();
        capturePositions[Color.Black] = new List<Position>();

        InitBoard();
    }

    private void InitBoard()
    {
        //for (int rank = 0; rank < BoardPositions.GetLength(0); rank++)
        //{
        //    for (int file = 0; file < BoardPositions.GetLength(1); file++)
        //    {
        //        BoardPositions[rank, file] = new BoardPosition(null, (rank + file) % 2 == 0 ? Color.Black : Color.White, new Position(rank, file));
        //    }
        //}

        for (int rank = 0; rank < MaxRank; rank++)
        {
            for (int file = 0; file < MaxFile; file++)
            {
                var pos = new Position(rank, file);
                boardPositions.Add(pos, new BoardPosition(null, (rank + file) % 2 == 0 ? Color.Black : Color.White, pos));
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

                //BoardPositions[rank, file].SetPiece(piece);
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
                    //BoardPositions[rook.Position.Rank, rook.Position.File + moveOffset].SetPiece(rook);
                    //BoardPositions[rook.Position.Rank, rook.Position.File].SetPiece(null);
                    boardPositions[new Position(rook.Position.Rank, rook.Position.File + moveOffset)].SetPiece(rook);
                    boardPositions[new Position(rook.Position.Rank, rook.Position.File)].SetPiece(null);
                }
            }
        }

        // Swap pieces
        //BoardPositions[e.StartPosition.Rank, e.StartPosition.File].SetPiece(null);
        //BoardPositions[e.EndPosition.Rank, e.EndPosition.File].SetPiece(piece);
        boardPositions[new Position(e.StartPosition.Rank, e.StartPosition.File)].SetPiece(null);
        boardPositions[new Position(e.EndPosition.Rank, e.EndPosition.File)].SetPiece(piece);


        // Re-calculate capture moves
        var opponentsKing = boardPositions.Single(x => x.Value.Piece is King && x.Value.Piece.Color != Game.Turn);

        // TODO: Find better solution
        var tmp = opponentsKing.Value.Piece;
        opponentsKing.Value.SetPiece(null);

        capturePositions[Game.Turn].Clear();
        foreach (var item in boardPositions)
        {
            if (item.Value.Piece is not null && item.Value.Piece.Color == Game.Turn)
            {
                capturePositions[Game.Turn].AddRange(item.Value.Piece.GetPossiblePositions().Select(x => x.EndPosition));
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
        opponentsKing.Value.SetPiece(tmp);

        // Check or Checkmate
        if (capturePositions[Game.Turn].Contains(opponentsKing.Value.Position))
        {
            e.ChangeType(MoveType.Check);
            
            var kingPositions = opponentsKing.Value.Piece?.GetPossiblePositions().Select(x => x.EndPosition);
            var result = kingPositions.Except(capturePositions[Game.Turn]);

            if (!result.Any())
            {
                e.ChangeType(MoveType.Checkmate);
            }
        }

        // Record completed move
        MoveHistory.Add(e.ToString());

        // Change turn
        Game.ChangeTurn(e);

        PrintBoard();
    }

    public Piece? GetPieceOnPosition(Position position)
    {
        //return BoardPositions[position.Rank, position.File].Piece;
        return boardPositions[new Position(position.Rank, position.File)].Piece;
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
        Console.WriteLine("--------");

        for (int rank = MaxRank - 1; rank >= 0; rank--)
        {
            for (int file = 0; file < MaxFile; file++)
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
