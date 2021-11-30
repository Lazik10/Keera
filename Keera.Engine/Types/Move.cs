namespace Keera.Engine.Types;

public enum MoveType
{
    Move, Capture
}

public class Move
{
    public Position Position { get; private set; }
    public MoveType Type { get; private set; }

    public Move(Position position, MoveType type)
    {
        Position = position;
        Type = type;
    }
}
