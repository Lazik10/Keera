using Keera.Engine.Types;

namespace Keera.Engine.Game;

public class Game
{
    public Board Chessboard { get; set; }
    public Player WhitePlayer { get; set; }
    public Player BlackPlayer { get; set; }
    public GameStatus Status { get; set; }
    public Color Turn { get; set; }

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

    public Game(long id, Player whitePlayer, Player blackPlayer)
    {
        GameId = id;
        Chessboard = new Board(this);
        WhitePlayer = whitePlayer;
        BlackPlayer = blackPlayer;
        Status = GameStatus.Prepared;
        Turn = Color.White;
    }

    public void ChangeTurn(Move move)
    {
        if (move.Type == MoveType.Checkmate)
        {
            Player winner = Turn == Color.White ? WhitePlayer : BlackPlayer;
            Player loser = Turn == Color.White ? BlackPlayer : WhitePlayer;
            EndGame(GameStatus.EndedByWin, winner, loser);
        }

        Turn = Turn == Color.White ? Color.Black : Color.White;

        OnTurnChanged?.Invoke(this, Turn);
    }

    public void EndGame(GameStatus gameStatus, Player winner, Player loser)
    {
        Status = gameStatus;

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
    }
}

