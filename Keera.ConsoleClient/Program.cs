using Keera.Engine.Board;
using Keera.Engine.Pieces;
using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keera.ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var board = new Board();

            board.LoadPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");



            var pawn = new Pawn(new Position(1, 1), Color.White);

            pawn.OnPieceMoved += Pawn_OnPieceMoved;

            pawn.MoveTo(new Position(2, 4));
            pawn.MoveTo(new Position(5, 1));
        }

        private static void Pawn_OnPieceMoved(object? sender, Position e)
        {
            var pawn = sender as Pawn;

            Console.WriteLine($"Piece {pawn?.GetType().Name} moved to position {e.File} {e.Rank}");
        }
    }
}