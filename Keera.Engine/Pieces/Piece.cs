using Keera.Engine.Game;
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

    protected Board Board;

    protected Action? PieceMoved;

    public event EventHandler<Move>? OnPieceMoved;

    public Piece(Position position, Color color, uint value, Board board)
    {
        Position = position;
        Color = color;
        Value = value;
        Board = board;
    }

    protected abstract List<Move> GetPossiblePositions();

    protected virtual bool TryAddPossibleMove(List<Move> possibleMoves, Position position)
    {
        var piece = Board.GetPieceOnPosition(position);
        if (piece == null)
        {
            possibleMoves.Add(new Move(position, MoveType.Move));
            return true;
        }
        else if (Color != piece.Color)
        {
            possibleMoves.Add(new Move(position, MoveType.Capture));
            return true;
        }

        return false;
    }

    protected bool CanMoveTo(Position position, out Move? move)
    {
        var possiblePosititons = GetPossiblePositions();

        move = possiblePosititons.Find(x => x.Position.Equals(position));

        return move != null;
    }

    public void MoveTo(Position position)
    {
        if (!CanMoveTo(position, out var move))
        {
            return;
        }

        Position = position;

        PieceMoved?.Invoke();
        OnPieceMoved?.Invoke(this, move);
    }
}
