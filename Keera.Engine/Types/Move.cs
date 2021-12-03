using Keera.Engine.Pieces;

namespace Keera.Engine.Types;

public enum MoveType
{
    Move, Capture
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

    public override string ToString()
    {
        return $"{Piece?.Code}{StartPosition}{(Type == MoveType.Capture ? "x" : "")}{CapturedPiece?.Code}{EndPosition}";
    }

    public static Move FromString(string moveString)
    {
        Position startPosition;
        Position endPosition;
        MoveType moveType;

        startPosition = Position.FromString(moveString.Substring(1, 2));
        moveType = moveString.Contains('x') ? MoveType.Capture : MoveType.Move;
        endPosition = moveType == MoveType.Capture ? Position.FromString(moveString.Substring(4, 2)) : Position.FromString(moveString.Substring(3, 2));

        var move = new Move(startPosition, endPosition, null, moveType);

        return move;
    }
}
