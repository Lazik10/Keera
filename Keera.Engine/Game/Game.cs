using Keera.Engine.Pieces;
using Keera.Engine.Types;
using System.Diagnostics;

namespace Keera.Engine.Game;

public class Game
{
    public Board Chessboard { get; set; }
    public GameStatus Status { get; set; }
    public Color Turn { get; set; }

    public Dictionary<Color, Stopwatch> Timers { get; set; }
    public Dictionary<Color, Player> Players { get; set; }

    private readonly Random _random = new();

    public int TimerMinutes { get; set; }

    public Timer GameTimer { get; set; }

    public event EventHandler<Color>? OnTurnChanged;

    public static long GameId { get; set; }

    public enum GameStatus
    {
        Prepared,
        Running,
        EndedByWin,
        EndedByDraw,
        UnknownState
    }

    public Game(long id, Player whitePlayer, Player blackPlayer, int timerMinutes)
    {
        GameId = id;
        Chessboard = new Board(this);

        Players = new Dictionary<Color, Player>
        {
            [Color.White] = whitePlayer,
            [Color.Black] = blackPlayer
        };

        Status = GameStatus.Prepared;
        Turn = Color.White;
        Timers = new Dictionary<Color, Stopwatch>
        {
            [Color.White] = new Stopwatch(),
            [Color.Black] = new Stopwatch()
        };
        TimerMinutes = timerMinutes;
        //GameTimer = new Timer(TimerCallback, null, 0, 1000);
    }

    public void TimerCallback(object? stateInfo)
    {
        var shouldEnd = Timers.Any(x => x.Value.ElapsedMilliseconds >= TimerMinutes * 1000 * 60);

        if (shouldEnd)
        {
            Timers.Select(x => x.Value).ToList().ForEach(x => x.Stop());

            var winnerColor = Timers.Where(x => x.Value.ElapsedMilliseconds >= TimerMinutes * 1000 * 60).First().Key;
            EndGame(GameStatus.EndedByWin, winnerColor, Color.All ^ winnerColor);
        }
    }

    public void Start()
    {
        Status = GameStatus.Running;

        if (Players[Color.White].Type == PlayerType.AI)
        {
            Chessboard.CalculateCapturePositions(Color.All ^ Turn);
            Chessboard.CalculatePossibleMoves(Turn);

            MakeTurn(Color.White);
        }
    }

    public void MakeTurn(Color playerColor)
    {
        if (Chessboard.PossibleMoves[playerColor].Count > 0)
        {
            Move? move;
            do
            {
                move = Chessboard.PossibleMoves[playerColor].Skip(_random.Next(Chessboard.PossibleMoves[playerColor].Count)).Take(1).First();
            } while (!Chessboard.IsValidMove(move));

            //Console.ReadLine();
            Thread.Sleep(1);
            move?.Piece?.MoveTo(move.EndPosition);
        }
        else
        {
            var piece = Chessboard.GetKing(playerColor);
            if (piece is King king && king.Checked == false)
            {
                EndGame(GameStatus.EndedByDraw, Color.All ^ Turn, Turn);
                return;
            }
            else
            {
                EndGame(GameStatus.EndedByWin, Color.All ^ Turn, Turn);
                return;
            }
        }
    }

    public void ChangeTurn(MoveType? moveType)
    {
        // Switch timers
        Timers[Turn].Stop();
        Turn ^= Color.All;
        Timers[Turn].Start();

        OnTurnChanged?.Invoke(this, Turn);

        //Chessboard.PrintBoard();
        Chessboard.CalculatePossibleMoves(Turn);

        if (Players[Turn].Type == PlayerType.AI)
        {
            MakeTurn(Turn);
        }
        else if (Players[Turn].Type == PlayerType.Human)
        {
            Console.WriteLine($"Current turn: {Turn}");
            string? input;
            do
            {
                input = Console.ReadLine();
            }
            while (!Chessboard.MovePiece(input));
        }
    }

    public void EndGame(GameStatus gameStatus, Color win, Color lose)
    {
        Status = gameStatus;

        Player winner = Players[win];
        Player loser = Players[lose];

        Timers[winner.Color].Stop();
        Timers[loser.Color].Stop();

        // Distribute Elo points
        if (Status == GameStatus.EndedByWin)
        {
            Elo.Update(winner, 1, loser, 0);
            Console.WriteLine($"Player {winner.Color} won");
        }
        else if (Status == GameStatus.EndedByDraw)
        { 
            Elo.Update(winner, 0.5f, loser, 0.5f);
            Console.WriteLine("Game ended with draw");
        }

        winner.TotalGamesPlayed++;
        loser.TotalGamesPlayed++;
    }
}

