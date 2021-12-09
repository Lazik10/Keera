using Keera.Engine.Types;

namespace Keera.Engine.Game;

public enum PlayerType
{
    Human,
    AI
}

public class Player
{
    public string Name { get; set; }
    public Color Color { get; set; }
    public PlayerType Type { get; set; }
    public float MatchScore { get; set; }

    public Elo? Rating { get; set; }
    public int? TotalGamesPlayed { get; set; }
    public int Age { get; set; }

    public Player(string name, Color color, PlayerType type, Elo? rating)
    { 
        Name = name;
        Color = color;
        Rating = rating;
        TotalGamesPlayed = 0;
        Age = 15;
        Type = type;
    }
}
