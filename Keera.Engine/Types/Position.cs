namespace Keera.Engine.Types;

public struct Position
{
    public int Rank { get; private set; }
    public int File { get; private set; }

    public Position(int rank, int file)
    {
        Rank = rank;
        File = file;
    }
}
