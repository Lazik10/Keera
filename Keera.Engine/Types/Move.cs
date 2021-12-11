using Keera.Engine.Pieces;

namespace Keera.Engine.Types;

[Flags]
public enum MoveType
{
    Move      = 0,
    MoveByTwo = 1,
    EnPassant = 2,
    Capture   = 4,
    CastlingQ = 8,
    CastlingK = 16,
    Check     = 32,
    Checkmate = 64
}

public class Move
{
    public Position StartPosition { get; private set; }
    public Position EndPosition { get; private set; }

    public Piece? Piece { get; private set; }
    public Piece? CapturedPiece { get; private set; }

    public MoveType Type { get; private set; }

    public Move(Position startPosition, Position endPosition, Piece? piece, MoveType type)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        Piece = piece;
        Type = type;
    }

    public Move(Position startPosition, Position endPosition, Piece? piece, MoveType type, Piece capturedPiece) : this(startPosition, endPosition, piece, type)
    {
        CapturedPiece = capturedPiece;
    }

    public void AddTypeFlag(MoveType type)
    {
        Type |= type;
    }

    public void RemoveTypeFlag(MoveType type)
    {
        Type &= ~type;
    }

    public override string ToString()
    {
        if (Type.HasFlag(MoveType.CastlingK))
            return "O-O";
        else if (Type.HasFlag(MoveType.CastlingQ))
            return "O-O-O";

        return $"{Piece?.Code}{StartPosition}" +
               $"{(Type.HasFlag(MoveType.Capture | MoveType.EnPassant) ? "x" : "")}" +
               $"{CapturedPiece?.Code}{EndPosition}" +
               $"{(Type.HasFlag(MoveType.Check) ? "+" : "")}" +
               $"{(Type.HasFlag(MoveType.Checkmate) ? "#" : "")}" +
               $"{(Type.HasFlag(MoveType.EnPassant) ? " e.p." : "")}";
    }

    public static Move FromString(string moveString, Game.Game game)
    {
        Position startPosition;
        Position endPosition;
        MoveType moveType;

        if (moveString == "O-O-O")
        {
            moveType = MoveType.CastlingQ;
            startPosition = game.Turn == Color.White ? new Position(0, 4) : new Position(7, 4);
            endPosition = game.Turn == Color.White ? new Position(0, 2) : new Position(7, 2);
        }
        else if (moveString == "O-O")
        {
            moveType = MoveType.CastlingK;
            startPosition = game.Turn == Color.White ? new Position(0, 4) : new Position(7, 4);
            endPosition = game.Turn == Color.White ? new Position(0, 6) : new Position(7, 6);
        }
        else
        {
            startPosition = Position.FromString(moveString.Substring(1, 2));
            moveType = moveString.Contains('x') ? MoveType.Capture : MoveType.Move;
            endPosition = moveType == MoveType.Capture ? Position.FromString(moveString.Substring(4, 2)) : Position.FromString(moveString.Substring(3, 2));
        }

        return new(startPosition, endPosition, null, moveType);
    }
}
