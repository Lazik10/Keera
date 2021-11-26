using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class Queen : Piece
{
    public Queen(Position position, Color color) : base(position, color, 9)
    {
    }

    protected override bool CanMoveTo(Position position)
    {
        throw new NotImplementedException();
    }
}
