using Keera.Engine.Pieces;

namespace Keera.Engine.Types;

internal struct BoardPosition
{
    public Piece? Piece { get; private set; }
    public Color Color { get; private set; }
    public Position Position { get; private set; }

    public BoardPosition(Piece? piece, Color color, Position position)
    {
        Piece = piece;
        Color = color;
        Position = position;
    }

    public void SetPiece(Piece? piece)
    {
        Piece = piece;
    }
}
