using System;
using System.Collections.Generic;

// WinChecker:
// - Starts from the last move and scans all directions (horizontal, vertical, and two diagonals)
// - Builds the path of consecutive cells for the player based on the last move
// - Detects a win if the line length is greater or equal to connectLength
public static class WinChecker
{
    // directions to scan from the last move: horizontal, vertical, and two diagonals
    private static readonly (int dRow, int dCol)[] Directions =
    {
        (0, 1),   // horizontal
        (1, 0),   // vertical
        (1, 1),   // diagonal down-right
        (1, -1)   // diagonal down-left
    };

    // Checks if the last move caused a win for the given player
    public static WinResult CheckForWin(
        BoardState board,
        BoardPosition lastMove,
        int playerId,
        int connectLength)
    {
        // iterating in every direction starting from the last move
        foreach (var (dRow, dCol) in Directions)
        {
            // building line of consecutive playerId cells in both directions
            var line = BuildLine(board, lastMove, playerId, dRow, dCol);

            // exiting if we found a winning line
            if (line.Count >= connectLength)
            {
                return WinResult.Win(line);
            }
        }

        return WinResult.NoWin();
    }

    // Builds a line of consecutive cells for playerId starting at 'start' and going both ways
    private static List<BoardPosition> BuildLine(
        BoardState board,
        BoardPosition start,
        int playerId,
        int dRow,
        int dCol)
    {
        var result = new List<BoardPosition> { start };

        // moving forward (+dRow, +dCol)
        int row = start.Row + dRow;
        int col = start.Column + dCol;

        while (IsInBounds(board, row, col) && board.GetCell(row, col) == playerId)
        {
            result.Add(new BoardPosition(row, col));
            row += dRow;
            col += dCol;
        }

        // moving backward (-dRow, -dCol)
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

    // Checks if (row, col) is inside the board boundaries
    private static bool IsInBounds(BoardState board, int row, int col)
    {
        return row >= 0 &&
               row < board.Rows &&
               col >= 0 &&
               col < board.Columns;
    }
}
