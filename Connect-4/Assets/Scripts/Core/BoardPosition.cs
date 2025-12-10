// BoardPosition:
// - Represents a cell by row and column on the board
// - Used for addressing BoardState cells and move history
public readonly struct BoardPosition
{
    public int Row { get; }
    public int Column { get; }

    // Initializes a new instance of the BoardPosition struct
    public BoardPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public override string ToString() => $"({Row}, {Column})";

    public override bool Equals(object obj)
    {
        return obj is BoardPosition other &&
               other.Row == Row &&
               other.Column == Column;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Row * 397) ^ Column;
        }
    }

    public static bool operator ==(BoardPosition a, BoardPosition b) =>
        a.Row == b.Row && a.Column == b.Column;

    public static bool operator !=(BoardPosition a, BoardPosition b) =>
        !(a == b);
}

