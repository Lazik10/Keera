using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class King : Piece
{
    public King(Position position, Color color, Board board) : base(position, color, 15, board)
    {
        movedFromStart = false;

        Code = color == Color.White ? 'K' : 'k';

        PieceMoved = () =>
        {
            movedFromStart = true;
        };
    }

    private bool movedFromStart;

    protected override List<Move> GetPossiblePositions()
    {
        possiblePositions.Clear();

        MovesScan(Direction.UP, Position, 1);
        MovesScan(Direction.DOWN, Position, 1);
        MovesScan(Direction.RIGHT, Position, 1);
        MovesScan(Direction.LEFT, Position, 1);
        MovesScan(Direction.LEFTUP, Position, 1);
        MovesScan(Direction.LEFTDOWN, Position, 1);
        MovesScan(Direction.RIGHTUP, Position, 1);
        MovesScan(Direction.RIGHTDOWN, Position, 1);
        if (!movedFromStart)
            GetPossibleCastlingMoves();

        return possiblePositions;
    }

    private void GetPossibleCastlingMoves()
    {
        // Castling Queen side
        Rook? rookQSide = (Rook?)Board.GetPieceOnPosition(Color == Color.White ? new Position(0, 0) : new Position(7, 0));
        if (rookQSide != null && rookQSide is Rook && !rookQSide.Moved())
        {
            if (CheckForPossibleCastling(Direction.QUEENSIDE) == true)
                possiblePositions.Add(new Move(Position, new Position(Position.Rank, Position.File - 2), null, MoveType.CastlingQ));
        }
        // Castling King side
        Rook? rookKSide = (Rook?)Board.GetPieceOnPosition(Color == Color.White ? new Position(0, 7) : new Position(7, 7));
        if (rookKSide != null && rookKSide is Rook && !rookKSide.Moved())
        {
            if (CheckForPossibleCastling(Direction.KINGSIDE) == true)
                possiblePositions.Add(new Move(Position, new Position(Position.Rank, Position.File + 2), null, MoveType.CastlingK));
        }
    }

    private bool CheckForPossibleCastling(Direction direction)
    {
        Piece? piece = null;
        int depth = direction == Direction.QUEENSIDE ? -3 : 2;

        for (; depth >= 1; depth--)
        {
            piece = Board.GetPieceOnPosition(new Position(Position.Rank, Position.File + depth));
            if (piece != null)
                return false;
        }
        return true;
    }
}
