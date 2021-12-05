using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class Rook : Piece
{
    public Rook(Position position, Color color, Board board) : base(position, color, 5, board)
    {
        movedFromStart = false;
        Code = color == Color.White ? 'R' : 'r';

        PieceMoved = () =>
        {
            movedFromStart = true;
        };
    }

    private bool movedFromStart;

    public bool Moved() { return movedFromStart; }

    protected override List<Move> GetPossiblePositions()
    {
        possiblePositions.Clear();

        MovesScan(Direction.UP, Position, Board.MaxRank);
        MovesScan(Direction.DOWN, Position, Board.MaxRank);
        MovesScan(Direction.RIGHT, Position, Board.MaxRank);
        MovesScan(Direction.LEFT, Position, Board.MaxRank);

        return possiblePositions;
    }
}
