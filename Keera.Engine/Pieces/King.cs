using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class King : Piece
{
    public bool Checked { get; set; }
    public King(Position position, Color color, Board board) : base(position, color, 15, board)
    {
        movedFromStart = false;

        Code = color == Color.White ? 'K' : 'k';

        PieceMoved = () =>
        {
            movedFromStart = true;
        };

        Checked = false;
    }

    private bool movedFromStart;

    public override List<Move> GetPossiblePositions()
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

        return possiblePositions.Where(x => !Board.capturePositions[Color.All ^ Color].Contains(x.EndPosition)).ToList();
    }

    private void GetPossibleCastlingMoves()
    {
        // Castling Queen side
        Piece? rookQSide = Board.GetPieceOnPosition(Color == Color.White ? new Position(0, 0) : new Position(7, 0));
        if (rookQSide != null && rookQSide is Rook rookQ && !rookQ.Moved())
        {
            if (CheckForPossibleCastling(Direction.QUEENSIDE) && !Board.capturePositions[Color.All ^ Color].Contains(rookQSide.Position))
                possiblePositions.Add(new Move(Position, new Position(Position.Rank, Position.File - 2), null, MoveType.Move | MoveType.CastlingQ));
        }
        // Castling King side
        Piece? rookKSide = Board.GetPieceOnPosition(Color == Color.White ? new Position(0, 7) : new Position(7, 7));
        if (rookKSide != null && rookKSide is Rook rookK && !rookK.Moved())
        {
            if (CheckForPossibleCastling(Direction.KINGSIDE) && !Board.capturePositions[Color.All ^ Color].Contains(rookKSide.Position))
                possiblePositions.Add(new Move(Position, new Position(Position.Rank, Position.File + 2), null, MoveType.Move | MoveType.CastlingK));
        }
    }

    private bool CheckForPossibleCastling(Direction direction)
    {
        Piece? piece = null;
        int depth = direction == Direction.QUEENSIDE ? 3 : 2;
        int sign = direction == Direction.QUEENSIDE ? -1 : 1;

        for (var i = 1; i <= depth; i++)
        {
            var pos = new Position(Position.Rank, Position.File + sign * i);

            piece = Board.GetPieceOnPosition(pos);
            if (piece != null || Board.capturePositions[Color.All ^ Color].Contains(pos))
                return false;
        }
        return true;
    }
}
