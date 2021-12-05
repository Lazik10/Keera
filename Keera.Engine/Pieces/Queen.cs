using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class Queen : Piece
{
    public Queen(Position position, Color color, Board board) : base(position, color, 9, board)
    {
        Code = color == Color.White ? 'Q' : 'q';
    }

    protected override List<Move> GetPossiblePositions()
    {
        possiblePositions.Clear();

        MovesScan(Direction.UP, Position, Board.MaxRank);
        MovesScan(Direction.DOWN, Position, Board.MaxRank);
        MovesScan(Direction.RIGHT, Position, Board.MaxRank);
        MovesScan(Direction.LEFT, Position, Board.MaxRank);
        MovesScan(Direction.LEFTUP, Position, Board.MaxRank);
        MovesScan(Direction.LEFTDOWN, Position, Board.MaxRank);
        MovesScan(Direction.RIGHTUP, Position, Board.MaxRank);
        MovesScan(Direction.RIGHTDOWN, Position, Board.MaxRank);

        return possiblePositions;
    }
}
