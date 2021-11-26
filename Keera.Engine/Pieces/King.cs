using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class King : Piece
{
    public King(Position position, Color color) : base(position, color, 15)
    {
    }

    protected override bool CanMoveTo(Position position)
    {
        throw new NotImplementedException();
    }
}
