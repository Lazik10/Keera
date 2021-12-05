using Keera.Engine.Pieces;

namespace Keera.Engine.Types;

public enum MoveType
{
    Move,
    Capture,
    CastlingQ,
    CastlingK
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
        if (Type == MoveType.CastlingK)
            return "O-O";
        else if (Type == MoveType.CastlingQ)
            return "O-O-O";

        return $"{Piece?.Code}{StartPosition}{(Type == MoveType.Capture ? "x" : "")}{CapturedPiece?.Code}{EndPosition}";
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

        Move move = new(startPosition, endPosition, null, moveType);

        return move;
    }
}
