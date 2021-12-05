using Keera.Engine.Types;

namespace Keera.Engine.Game;

public class Player
{
    public string Name { get; set; }
    public Color Color { get; set; }
    public Elo Rating { get; set; }

    public Player(string name, Color color, Elo rating)
    { 
        Name = name;
        Color = color;
        Rating = rating;
    }
}
