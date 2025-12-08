using System;
using UnityEngine;

// Board state:
// - Holds the grid, size, and turn counter
// - Checks if a column can accept a piece, plays a move, and sees if the board is full
public class BoardState
{
    private readonly int[,] _grid;

    public int Rows { get; }
    public int Columns { get; }
    public int TurnCount { get; private set; }

    public BoardState(int rows, int columns)
    {
        if (rows <= 0)
        {
            Debug.LogWarning($"[BoardState]: rows <= 0 ({rows}). Using rows = 1 instead.");
            rows = 1;
        }

        if (columns <= 0)
        {
            Debug.LogWarning($"[BoardState]: columns <= 0 ({columns}). Using columns = 1 instead.");
            columns = 1;
        }

        Rows = rows;
        Columns = columns;
        _grid = new int[Rows, Columns];
        TurnCount = 0;
    }

    // Returns the playerId at (row, column) - 0 = empty
    public int GetCell(int row, int column)
    {
        return _grid[row, column];
    }

    // Returns true if a piece can be placed in this column
    public bool CanPlay(int column)
    {
        if (column < 0 || column >= Columns)
            return false;

        // column is full if the top cell is not zero
        return _grid[Rows - 1, column] == 0;
    }

    // Tries to play a move for the given player in the given column
    // Returns a MoveResult with info about success and the placed cell
    public MoveResult PlayMove(int column, int playerId)
    {
        if (!CanPlay(column))
        {
            Debug.Log($"[BoardState.PlayMove]: Cannot play in column {column}. Column out of bounds or full.");
            return MoveResult.Invalid();
        }

        int targetRow = GetNextEmptyRow(column);
        if (targetRow < 0)
        {
            Debug.Log($"[BoardState.PlayMove]: No empty row found in column {column}.");
            return MoveResult.Invalid();
        }

        _grid[targetRow, column] = playerId;
        TurnCount++;

        return MoveResult.Successful(playerId, targetRow, column);
    }

    // Returns true if the board is full (no more possible moves)
    public bool IsFull()
    {
        // board is not full if any cell in the top row is zero
        for (int col = 0; col < Columns; col++)
        {
            if (_grid[Rows - 1, col] == 0)
                return false;
        }

        return true;
    }

    // Finds the next empty row in the given column
    private int GetNextEmptyRow(int column)
    {
        // Start from bottom row (0) upwards.
        for (int row = 0; row < Rows; row++)
        {
            if (_grid[row, column] == 0)
            {
                return row;
            }
        }

        // should never happen if CanPlay was checked before!!
        Debug.LogWarning($"[BoardState.GetNextEmptyRow]: No empty row found in column {column}.");
        return -1;
    }
}