// Move result:
// - Indicates if a move was applied successfully
// - Stores which player made the move and where it landed
public readonly struct MoveResult
{
    public bool Success { get; }
    public int PlayerId { get; }
    public BoardPosition Position { get; }

    // Private constructor used by methods below
    private MoveResult(bool success, int playerId, BoardPosition position)
    {
        Success = success;
        PlayerId = playerId;
        Position = position;
    }

    // Creates an invalid move result (no player and no valid position) 
    public static MoveResult Invalid() => new MoveResult(false, 0, new BoardPosition(-1, -1));

    // Creates a successful move result for the given player and cell
    public static MoveResult Successful(int playerId, int row, int column) => new MoveResult(true, playerId, new BoardPosition(row, column));
}
