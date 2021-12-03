using Keera.Engine.Game;
using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class Bishop : Piece
{
    public Bishop(Position position, Color color, Board board) : base(position, color, 3, board)
    {
        Code = color == Color.White ? 'B' : 'b';
    }

    protected override List<Move> GetPossiblePositions()
    {
        possiblePositions.Clear();

        MovesScan(Direction.LEFTUP, Position, Board.MaxRank);
        MovesScan(Direction.LEFTDOWN, Position, Board.MaxRank);
        MovesScan(Direction.RIGHTUP, Position, Board.MaxRank);
        MovesScan(Direction.RIGHTDOWN, Position, Board.MaxRank);

        return possiblePositions;
    }
}
