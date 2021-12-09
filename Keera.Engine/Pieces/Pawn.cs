using Keera.Engine.Game;
using Keera.Engine.Types;

namespace Keera.Engine.Pieces;

public class Pawn : Piece
{
    public Pawn(Position position, Color color, Board board) : base(position, color, 1, board)
    {
        movedFromStart = false;
        MovedByTwo = false;
        Code = color == Color.White ? 'P' : 'p';

        OnPieceMoved += Pawn_MovedByTwo;
        board.Game.OnTurnChanged += Pawn_ResetMovedByTwo;

        PieceMoved = () =>
        {
            movedFromStart = true;
        };
    }

    private bool movedFromStart;

    public bool MovedByTwo { get; set; }

    public override List<Move> GetPossiblePositions()
    {
        var possiblePositions = new List<Move>();

        var sign = Color == Color.White ? 1 : -1;

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

        return possiblePositions;
    }

    protected override bool TryAddPossibleMove(List<Move> possibleMoves, Position position)
    {
        if (!IsValidBoardPosition(position))
            return false;

        var piece = Board.GetPieceOnPosition(position);
        if (piece == null)
        {
            if (position.File == Position.File)
            {
                possibleMoves.Add(new Move(Position, position, this, MoveType.Move));
                return true;
            }
            else
            {
                possibleMoves.Add(new Move(Position, position, this, MoveType.Capture));

                var sign = Color == Color.White ? 1 : -1;

                Position neighbourPos = new (Board.MaxRank, Board.MaxFile);
                Piece? neighbourPiece = null;

                // Right side
                if (position.File == Position.File - sign)
                {
                    neighbourPos = new Position(Position.Rank, Position.File - (1 * sign));
                }
                // Left side
                else if (position.File == Position.File + sign)
                {
                    neighbourPos = new Position(Position.Rank, Position.File + (1 * sign));
                }

                if (IsValidBoardPosition(neighbourPos))
                {
                    neighbourPiece = Board?.GetPieceOnPosition(neighbourPos);

                    if (neighbourPiece is Pawn pawn && pawn.MovedByTwo == true && !pawn.Color.Equals(Color))
                    {
                        possibleMoves.Add(new Move(Position, position, this, MoveType.EnPassant | MoveType.Move | MoveType.Capture));
                        return true;
                    }
                }
                return false;
            }
        }
        else if (Color != piece.Color && position.File != Position.File)
        {
            possibleMoves.Add(new Move(Position, position, this, MoveType.Capture | MoveType.Move, piece));
            return true;
        }

        return false;
    }

    private void Pawn_MovedByTwo(object? sender, Move e)
    {
        if (Math.Abs(e.StartPosition.Rank - e.EndPosition.Rank) == 2)
        {
            MovedByTwo = true;
        }
    }

    private void Pawn_ResetMovedByTwo(object? sender, Color turn)
    {
        if (MovedByTwo == true && turn == Color)
        {
            MovedByTwo = false;
        }
    }

    public static Piece Promote(Pawn promotedPawn, Move move)
    {
        Piece? piece;
        Console.WriteLine("Set char for promoting piece type: ");
        do
        {
            _ = char.TryParse(Console.ReadLine(), out char ch);
            piece = ch switch
            {
                'n' => new Knight(move.EndPosition, promotedPawn.Color, promotedPawn.Board),
                'b' => new Bishop(move.EndPosition, promotedPawn.Color, promotedPawn.Board),
                'r' => new Rook(move.EndPosition, promotedPawn.Color, promotedPawn.Board),
                'q' => new Queen(move.EndPosition, promotedPawn.Color, promotedPawn.Board),
                _ => null
            };

        } while (piece == null);

        return piece;
    }
}
