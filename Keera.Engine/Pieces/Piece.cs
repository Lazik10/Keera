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
    public char Code { get; set; }

    protected List<Move> possiblePositions;

    protected Board Board;

    protected Action? PieceMoved;

    public event EventHandler<Move>? OnPieceMoved;

    public Piece(Position position, Color color, uint value, Board board)
    {
        Position = position;
        Color = color;
        Value = value;
        Board = board;

        possiblePositions = new List<Move>();
    }

    protected abstract List<Move> GetPossiblePositions();

    protected virtual bool TryAddPossibleMove(List<Move> possibleMoves, Position position)
    {
        var piece = Board.GetPieceOnPosition(position);
        if (piece == null)
        {
            possibleMoves.Add(new Move(Position, position, this, MoveType.Move));
            return true;
        }
        else if (Color != piece.Color)
        {
            possibleMoves.Add(new Move(Position, position, this, MoveType.Capture, piece));
            return true;
        }

        return false;
    }

    protected bool IsValidBoardPosition(Position position)
    {
        if (position.Rank < Board.MaxRank && position.Rank >= 0
            && position.File < Board.MaxFile && position.File >= 0)
            return true;
        return false;
    }

    protected List<Move> MovesScan(Direction dir, Position position, int depth)
    {
        position = dir switch
        {
            Direction.LEFT      => new Position(position.Rank, position.File - 1),
            Direction.RIGHT     => new Position(position.Rank, position.File + 1),
            Direction.UP        => new Position(position.Rank + 1, position.File),
            Direction.DOWN      => new Position(position.Rank - 1, position.File),
            Direction.LEFTUP    => new Position(position.Rank + 1, position.File - 1),
            Direction.LEFTDOWN  => new Position(position.Rank - 1, position.File - 1),
            Direction.RIGHTUP   => new Position(position.Rank + 1, position.File + 1),
            Direction.RIGHTDOWN => new Position(position.Rank - 1, position.File + 1),
            _ => throw new Exception("Unsupported direction")
        };

        if (!IsValidBoardPosition(position))
            return possiblePositions;
        else
        {
            bool result = TryAddPossibleMove(possiblePositions, position);

            if (!result) // Can't go further because my piece is in the way 
                return possiblePositions;
            else
            {
                var lastmove = possiblePositions.Last();
                if (lastmove.Type == MoveType.Capture) // Can't go further because enemy piece is in the way
                    return possiblePositions;
            }
        }

        if (--depth > 0)
            MovesScan(dir, position, depth);

        return possiblePositions;
    }

    protected bool CanMoveTo(Position position, out Move? move)
    {
        var possiblePosititons = GetPossiblePositions();

        move = possiblePosititons.Find(x => x.EndPosition.Equals(position));

        return move != null;
    }

    public void MoveTo(Position position)
    {
        if (!CanMoveTo(position, out var move) || move == null)
        {
            return;
        }

        OnPieceMoved?.Invoke(this, move);

        Position = position;

        PieceMoved?.Invoke();
    }

    public void PrintAvailableMovePositions()
    {
        GetPossiblePositions();

        Console.WriteLine($"Number of available moves: { possiblePositions.Count }");
        foreach (Move move in possiblePositions)
        {
            Console.WriteLine($"Move: { move.EndPosition.Rank} { move.EndPosition.File}");
        }
    }
}
