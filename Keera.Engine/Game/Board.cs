using Keera.Engine.Pieces;
using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Game;

public class Board
{
    private readonly BoardPosition[,] BoardPositions;
    private readonly List<string> MoveHistory;

    public const int MaxFile = 8;
    public const int MaxRank = 8;

    public Board()
    {
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
        var piece = sender as Piece;

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

        var move = Move.FromString(moveString);

        var piece = GetPieceOnPosition(move.StartPosition);

        if (piece != null)
        {
            piece.MoveTo(move.EndPosition);
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
        Console.WriteLine($"Move history:");
        foreach (var move in MoveHistory)
        {
            Console.WriteLine(move);
        }
    }
}
