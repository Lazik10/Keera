using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class Pawn : Piece
{
    public Pawn(Position position, Color color, Board board) : base(position, color, 1, board)
    {
        movedFromStart = false;
        Code = color == Color.White ? 'P' : 'p';

        PieceMoved = () =>
        {
            movedFromStart = true;
        };
    }

    private bool movedFromStart;

    public override List<Move> GetPossiblePositions()
    {
        var possiblePositions = new List<Move>();

        var sign = Color == Color.White ? 1 : -1;
        var rankBounds = Color == Color.White ? Position.Rank < Board.MaxRank - 1 : Position.Rank > 0;

        if (rankBounds)
        {
            var position = new Position(Position.Rank + sign, Position.File);

            if (TryAddPossibleMove(possiblePositions, position))
            {
                if (!movedFromStart)
                {
                    position = new Position(Position.Rank + (Color == Color.White ? 2 : -2), Position.File);
                    TryAddPossibleMove(possiblePositions, position);
                }
            }

            if (Position.File > 0)
            {
                position = new Position(Position.Rank + sign, Position.File - 1);
                TryAddPossibleMove(possiblePositions, position);
            }
            if (Position.File < Board.MaxFile - 1)
            {
                position = new Position(Position.Rank + sign, Position.File + 1);
                TryAddPossibleMove(possiblePositions, position);
            }
        }

        return possiblePositions;
    }

    protected override bool TryAddPossibleMove(List<Move> possibleMoves, Position position)
    {
        if (!IsValidBoardPosition(position))
            return false;

        var piece = Board.GetPieceOnPosition(position);
        if (piece == null)
        {
            possibleMoves.Add(new Move(Position, position, this, MoveType.Move));
            return true;
        }
        else if (Color != piece.Color && position.File != Position.File)
        {
            possibleMoves.Add(new Move(Position, position, this, MoveType.Capture, piece));
            return true;
        }

        return false;
    }
}
