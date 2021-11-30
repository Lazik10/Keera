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



            var pawn = board.GetPieceOnPosition(new Position(1, 0));

            pawn.OnPieceMoved += Pawn_OnPieceMoved;

            pawn.MoveTo(new Position(5, 5));

            pawn.MoveTo(new Position(3, 0));
            pawn.MoveTo(new Position(4, 0));
            pawn.MoveTo(new Position(5, 0));
            pawn.MoveTo(new Position(6, 0));
            pawn.MoveTo(new Position(6, 1));
        }

        private static void Pawn_OnPieceMoved(object? sender, Move e)
        {
            var pawn = sender as Pawn;

            Console.WriteLine($"Piece {pawn?.GetType().Name} moved to position {e.Position.File} {e.Position.Rank}");
        }
    }
}