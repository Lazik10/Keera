using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public class Rook : Piece
{
    public Rook(Position position, Color color) : base(position, color, 5)
    {
    }

    protected override bool CanMoveTo(Position position)
    {
        throw new NotImplementedException();
    }
}
