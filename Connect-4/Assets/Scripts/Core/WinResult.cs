using System.Collections.Generic;

// WinResult:
// - Indicates if the last win check resulted in a win
// - Holds the positions that make the winning streak when there is a win
public readonly struct WinResult
{
    public bool IsWin { get; }
    public List<BoardPosition> WinningPositions { get; }

    // Private constructor used by factory methods
    private WinResult(bool isWin, List<BoardPosition> winningPositions)
    {
        IsWin = isWin;
        WinningPositions = winningPositions ?? new List<BoardPosition>();
    }

    // Creates a non-winning result (empty positions)
    public static WinResult NoWin() => new WinResult(false, new List<BoardPosition>());

    // Creates a winning result with a list of positions
    public static WinResult Win(List<BoardPosition> winningPositions) => new WinResult(true, winningPositions);
}
