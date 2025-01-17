﻿using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Player whitePlayer = new("Kurunzo", Color.White, PlayerType.AI, new Elo());
            Player blackPlayer = new("Lazik", Color.Black, PlayerType.AI, new Elo());

            Game game = new(0, whitePlayer, blackPlayer, 3);
            
            // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR
            //game.Chessboard.LoadPosition("r3k3/ppppppp1/2P5/8/8/8/PPPPPPP1/R3K2R");
            game.Chessboard.LoadPosition("k7/8/8/8/8/8/7p/7K");
            game.Chessboard.PrintBoard();

            game.Start();

            //string? input;
            //do
            //{
            //    Console.WriteLine($"Current turn: {game.Turn}");
            //    input = Console.ReadLine();

            //    if (input == "q")
            //    {
            //        game.EndGame(Game.GameStatus.EndedByWin, whitePlayer, blackPlayer);
            //        break;
            //    }

            //    game.Chessboard.MovePiece(input);
            //    game.Chessboard.PrintMoveHistory();

            //} while (game.Status == Game.GameStatus.Running);
        }
    }
}