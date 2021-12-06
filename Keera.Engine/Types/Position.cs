namespace Keera.Engine.Types;

public struct Position : IEquatable<Position>
{
    public int Rank { get; private set; }
    public int File { get; private set; }

    public Position(int rank, int file)
    {
        Rank = rank;
        File = file;
    }

    public override string ToString()
    {
        return $"{(char)(File + 97)}{Rank + 1}";
    }

    public static Position FromString(string positionString)
    {
        var rank = int.Parse(positionString[1].ToString()) - 1;
        var file = positionString[0] - 'a';

        return new Position(rank, file);
    }

    public bool Equals(Position other)
    {
        return Rank == other.Rank && File == other.File;
    }
}
