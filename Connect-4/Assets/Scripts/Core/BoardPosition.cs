// Board coordinates representing a cell by row and column
public readonly struct BoardPosition
{
    public int Row { get; }
    public int Column { get; }

    // Initializes a new instance of the "BoardPosition" struct
    public BoardPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public override string ToString() => $"({Row}, {Column})";
}

