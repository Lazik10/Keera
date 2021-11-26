using Keera.Engine.Pieces;
using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Board
{
    public class Board
    {
        private readonly BoardPosition[,] BoardPositions;

        public Board()
        {
            BoardPositions = new BoardPosition[8, 8];

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

            var rank = 0;
            var file = 0;

            foreach (var p in position)
            {
                if (p == '/')
                {
                    rank++;
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
                        'p' => new Pawn(piecePosition, pieceColor),
                        'n' => new Knight(piecePosition, pieceColor),
                        'b' => new Bishop(piecePosition, pieceColor),
                        'r' => new Rook(piecePosition, pieceColor),
                        'q' => new Queen(piecePosition, pieceColor),
                        'k' => new King(piecePosition, pieceColor),
                        _ => throw new ArgumentException("Unsupported piece character")
                    };

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
    }
}
