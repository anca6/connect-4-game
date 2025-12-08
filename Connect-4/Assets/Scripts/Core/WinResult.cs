using System.Collections.Generic;

// Win result:
// - Indicates if the last check found is a win
// - Holds the positions that make the winning streak when there is a win
public readonly struct WinResult
{
    public bool IsWin { get; }
    public List<BoardPosition> WinningPositions { get; }

    public WinResult(bool isWin, List<BoardPosition> winningPositions)
    {
        IsWin = isWin;
        WinningPositions = winningPositions;
    }

    // Creates a non-winning result (no positions)
    public static WinResult NoWin() => new WinResult(false, null);
}
