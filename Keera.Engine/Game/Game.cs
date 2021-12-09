using Keera.Engine.Pieces;
using Keera.Engine.Types;
using System.Diagnostics;
using System.Linq;

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
            EndGame(GameStatus.EndedByWin, Players[winnerColor], Players[winnerColor == Color.White ? Color.Black : Color.Black]);
        }
    }

    public void Start()
    {
        Status = GameStatus.Running;

        if (Players[Color.White].Type == PlayerType.AI)
        {
            Chessboard.CalculateCapturePositions(Turn == Color.White ? Color.Black : Color.White);
            Chessboard.CalculatePossibleMoves(Turn, null);

            MakeTurn(null);
        }
    }

    public void MakeTurn(MoveType? moveType)
    {
        var move = Chessboard.PossibleMoves[Turn].Skip(_random.Next(Chessboard.PossibleMoves[Turn].Count - 1)).Take(1).First();

        Console.WriteLine($"PM: {Chessboard.PossibleMoves[Turn].Count}");
        Console.WriteLine($"KPM: {Chessboard.PossibleMoves[Turn].Where(x => x.Piece is King).ToList().Count}");

        //Thread.Sleep(10);

        move.Piece.MoveTo(move.EndPosition);
    }

    public void ChangeTurn(MoveType? moveType)
    {
        if (moveType == MoveType.Checkmate)
        {
            EndGame(GameStatus.EndedByWin, Players[Turn], Players[Turn == Color.White ? Color.Black : Color.Black]);
            return;
        }

        Timers[Turn].Stop();

        Turn = Turn == Color.White ? Color.Black : Color.White;

        Timers[Turn].Start();

        OnTurnChanged?.Invoke(this, Turn);

        if (Players[Turn].Type == PlayerType.AI)
        {
            MakeTurn(moveType);
        }
    }

    public void EndGame(GameStatus gameStatus, Player winner, Player loser)
    {
        Status = gameStatus;
        
        Timers[winner.Color].Stop();
        Timers[loser.Color].Stop();

        // Distribute Elo points
        if (Status == GameStatus.EndedByWin)
        {
            Elo.Update(winner, 1, loser, 0);
        }
        else if (Status == GameStatus.EndedByDraw)
        { 
            Elo.Update(winner, 0.5f, loser, 0.5f);
        }

        winner.TotalGamesPlayed++;
        loser.TotalGamesPlayed++;

        Console.WriteLine($"Player {winner.Color} won");
    }
}

