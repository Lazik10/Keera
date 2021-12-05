using Keera.Engine.Types;

namespace Keera.Engine.Game;

public class Game
{
    public Board Chessboard { get; set; }
    public Player WhitePlayer { get; set; }
    public Player BlackPlayer { get; set; }
    public GameStatus Status { get; set; }
    public Color Turn { get; set; }

    public static long GameId { get; set; }

    public enum GameStatus
    { 
        prepared,
        running,
        ended
    }

    public Game(long id, Player whitePlayer, Player blackPlayer)
    {
        GameId = id;
        Chessboard = new Board(this);
        WhitePlayer = whitePlayer;
        BlackPlayer = blackPlayer;
        Status = GameStatus.prepared;
        Turn = Color.White;
    }
}

