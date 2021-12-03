using Keera.Engine.Game;
using Keera.Engine.Pieces;
using Keera.Engine.Types;

namespace Keera.ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var board = new Board();

            board.LoadPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");

            board.PrintBoard();

            string? input;
            do
            {
                input = Console.ReadLine();
                board.MovePiece(input);

            } while (input != "q");

            //var pawn = board.GetPieceOnPosition(new Position(1, 1));
            //Bishop bishop = (Bishop)board.GetPieceOnPosition(new Position(4, 0));
            //if (bishop != null && bishop is Bishop)
            //    bishop.PrintAvailableMovePositions();

            //if (pawn != null)
            //    pawn.OnPieceMoved += Pawn_OnPieceMoved;

            //pawn.MoveTo(new Position(5, 5));

            //pawn.MoveTo(new Position(3, 1));
            //pawn.MoveTo(new Position(4, 1));
            //pawn.MoveTo(new Position(5, 1));
            //pawn.MoveTo(new Position(6, 1));
            //pawn.MoveTo(new Position(6, 2));

            //board.PrintMoveHistory();
        }

        private static void Pawn_OnPieceMoved(object? sender, Move e)
        {
            var pawn = sender as Pawn;

            Console.WriteLine($"Piece {pawn?.GetType().Name} moved to position {e.EndPosition.File} {e.EndPosition.Rank} ({e.EndPosition})");
        }
    }
}