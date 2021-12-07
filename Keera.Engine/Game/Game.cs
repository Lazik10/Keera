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
        Ended
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
            EndGame();
        }

        Turn = Turn == Color.White ? Color.Black : Color.White;

        OnTurnChanged?.Invoke(this, Turn);
    }

    private void EndGame()
    {
        Status = GameStatus.Ended;
    }
}

