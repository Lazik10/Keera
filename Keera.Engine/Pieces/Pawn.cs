using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class Pawn : Piece
{
    public Pawn(Position position, Color color) : base(position, color, 1) { }

    protected override bool CanMoveTo(Position position)
    {
        return true;
    }
}
