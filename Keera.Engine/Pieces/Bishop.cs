using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class Bishop : Piece
{
    public Bishop(Position position, Color color) : base(position, color, 3)
    {
    }

    protected override bool CanMoveTo(Position position)
    {
        throw new NotImplementedException();
    }
}
