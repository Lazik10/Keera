using Keera.Engine.Game;
using Keera.Engine.Pieces;
using Keera.Engine.Types;

namespace Keera.ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Player whitePlayer = new("Kurunzo", Color.White, new Elo());
            Player blackPlayer = new("Lazik", Color.Black, new Elo());

            Game game = new(0, whitePlayer, blackPlayer);
            game.Status = Game.GameStatus.running;
            game.Chessboard.LoadPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R");
            game.Chessboard.PrintBoard();

            string? input;
            do
            {
                Console.WriteLine($"Current turn: {game.Turn}");
                input = Console.ReadLine();
                game.Chessboard.MovePiece(input);
                game.Chessboard.PrintMoveHistory();

            } while (input != "q" && game.Status == Game.GameStatus.running);

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