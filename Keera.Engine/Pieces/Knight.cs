using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class Knight : Piece
{
    public Knight(Position position, Color color, Board board) : base(position, color, 3, board)
    {
        Code = color == Color.White ? 'N' : 'n';
    }

    public override List<Move> GetPossiblePositions()
    {
        possiblePositions.Clear();

        // Up and right, Up and left
        MovesScan(Direction.RIGHT, new Position(this.Position.Rank + 2, this.Position.File), 1);
        MovesScan(Direction.LEFT, new Position(this.Position.Rank + 2, this.Position.File), 1);
        // Down and right, Down and left
        MovesScan(Direction.RIGHT, new Position(this.Position.Rank - 2, this.Position.File), 1);
        MovesScan(Direction.LEFT, new Position(this.Position.Rank - 2, this.Position.File), 1);
        // Right and up, Right and down
        MovesScan(Direction.UP, new Position(this.Position.Rank, this.Position.File + 2), 1);
        MovesScan(Direction.DOWN, new Position(this.Position.Rank, this.Position.File + 2), 1);
        // Left up, Left down
        MovesScan(Direction.UP, new Position(this.Position.Rank, this.Position.File - 2), 1);
        MovesScan(Direction.DOWN, new Position(this.Position.Rank, this.Position.File - 2), 1);

        return possiblePositions;
    }
}
