using Keera.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keera.Engine.Pieces;

public abstract class Piece
{
    public Position Position { get; private set; }
    public Color Color { get; init; }
    public uint Value { get; init; }


    public event EventHandler<Position>? OnPieceMoved;

    public Piece(Position position, Color color, uint value)
    {
        Position = position;
        Color = color;
        Value = value;
    }

    protected abstract bool CanMoveTo(Position position);
    public void MoveTo(Position position)
    {
        if (!CanMoveTo(position))
        {
            return;
        }

        Position = position;

        OnPieceMoved?.Invoke(this, Position);
    }
}
