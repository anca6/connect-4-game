using UnityEngine;

// GameConfigurator:
// - Holds board size (rows, columns) and required connect length for winning
// - Defines number of players
// - ScriptableObject so it can be created and tuned in the Editor
[CreateAssetMenu(fileName = "GameConfig", menuName = "Connect4/Game Configurator")]
public class GameConfigurator : ScriptableObject
{
    [Header("Board Settings")]
    [Tooltip("Number of rows in the board")]
    [Min(1)]
    public int rows = 6;

    [Tooltip("Number of columns in the board")]
    [Min(1)]
    public int columns = 7;

    [Tooltip("Number of connected pieces required to win")]
    [Min(2)]
    public int connectLength = 4;

    [Header("Players")]
    [Tooltip("Number of players in the game")]
    [Min(1)]
    public int playerCount = 2;
}
