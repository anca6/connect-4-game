using System.Collections.Generic;

// Win checking:
// - Starts from the last move and scans all directions (horizontal, vertical, and two diagonals)
// - Builds the player moves' path based on the last move
// - Detects win if the line length equals to connectLength
public static class WinChecker
{
    public static WinResult CheckForWin(
        BoardState board,
        BoardPosition lastMove,
        int playerId,
        int connectLength)
    {
        // directions to scan from the last move
        var directions = new (int dRow, int dCol)[]
        {
            (0, 1),   // horizontal
            (1, 0),   // vertical
            (1, 1),   // diagonal down-right
            (1, -1)   // diagonal down-left
        };

        foreach (var (dRow, dCol) in directions)
        {
            // building line of consecutive playerId cells
            var line = AddLine(board, lastMove, playerId, dRow, dCol);

            // exiting if we found a winning line
            if (line.Count >= connectLength)
            {
                return new WinResult(true, line);
            }
        }

        return WinResult.NoWin();
    }

    // Collects consecutive cells for playerId starting at 'start' and going both ways
    private static List<BoardPosition> AddLine(
        BoardState board,
        BoardPosition start,
        int playerId,
        int dRow,
        int dCol)
    {
        var result = new List<BoardPosition> { start };

        // forward (+dRow, +dCol)
        int row = start.Row + dRow;
        int col = start.Column + dCol;

        while (IsInBounds(board, row, col) && board.GetCell(row, col) == playerId)
        {
            result.Add(new BoardPosition(row, col));
            row += dRow;
            col += dCol;
        }

        // backward (-dRow, -dCol)
        row = start.Row - dRow;
        col = start.Column - dCol;

        while (IsInBounds(board, row, col) && board.GetCell(row, col) == playerId)
        {
            result.Add(new BoardPosition(row, col));
            row -= dRow;
            col -= dCol;
        }

        return result;
    }

    // Safety check for board boundaries
    private static bool IsInBounds(BoardState board, int row, int col)
    {
        return row >= 0 && row < board.Rows && col >= 0 && col < board.Columns;
    }
}
