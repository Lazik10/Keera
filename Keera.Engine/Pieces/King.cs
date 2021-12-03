using Keera.Engine.Game;
using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class King : Piece
{
    public King(Position position, Color color, Board board) : base(position, color, 15, board)
    {
        Code = color == Color.White ? 'K' : 'k';
    }

    protected override List<Move> GetPossiblePositions()
    {
        possiblePositions.Clear();

        MovesScan(Direction.UP, Position, 1);
        MovesScan(Direction.DOWN, Position, 1);
        MovesScan(Direction.RIGHT, Position, 1);
        MovesScan(Direction.LEFT, Position, 1);
        MovesScan(Direction.LEFTUP, Position, 1);
        MovesScan(Direction.LEFTDOWN, Position, 1);
        MovesScan(Direction.RIGHTUP, Position, 1);
        MovesScan(Direction.RIGHTDOWN, Position, 1);

        return possiblePositions;
    }
}
