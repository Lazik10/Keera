using Keera.Engine.Game;
using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class Rook : Piece
{
    public Rook(Position position, Color color, Board board) : base(position, color, 5, board)
    {
    }

    protected override List<Move> GetPossiblePositions()
    {
        var possiblePositions = new List<Move>();

        return possiblePositions;
    }
}
