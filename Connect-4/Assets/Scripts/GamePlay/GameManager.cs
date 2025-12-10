using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager:
// - Global singleton that persists across scenes
// - Holds global config and scene names
// - Owns BoardState, current player, game over flag, and move history
// - Controls global GameLogger setting
// - Entry points for starting a new game, loading a game, returning to menu and quitting
public class GameManager : MonoBehaviour
{
    #region Fields

    public static GameManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private GameConfigurator config;
    public GameConfigurator Config => config;

    [Header("Scenes")]
    [SerializeField] private string startSceneName = "StartScene";
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Debug")]
    [SerializeField] private bool enableLogging = true;

    // Game state that should persist between scenes
    public BoardState BoardState { get; private set; }
    public int CurrentPlayerId { get; set; } = 1;
    public bool IsGameOver { get; set; } = false;

    public Stack<MoveResult> MoveHistory { get; } = new Stack<MoveResult>();

    public bool HasActiveGame => BoardState != null;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // setting global logging flag based on inspector
        GameLogger.IsEnabled = enableLogging;
        GameLogger.Log("[GameManager]: Initialized and marked as DontDestroyOnLoad.");
    }

    #endregion

    #region Game State Management

    // Starts a new game by creating a fresh BoardState and resetting state
    public void StartNewGame()
    {
        if (config == null)
        {
            GameLogger.LogError("[GameManager.StartNewGame]: GameConfigurator not assigned.");
            return;
        }

        BoardState = new BoardState(config.rows, config.columns);
        CurrentPlayerId = 1;
        IsGameOver = false;
        MoveHistory.Clear();

        GameLogger.Log(
            $"[GameManager.StartNewGame]: New game created with {config.rows}x{config.columns} board.");
    }

    // Starts a new game and loads the game scene
    public void StartNewGameAndLoad()
    {
        StartNewGame();
        SceneManager.LoadScene(gameSceneName);
    }

    // Loads the game scene, assuming there is already an active BoardState
    public void LoadGame()
    {
        if (!HasActiveGame)
        {
            GameLogger.LogWarning(
                "[GameManager.LoadGame]: No active game to load. Start a new game first.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // Returns to main menu scene without clearing current game state
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(startSceneName);
    }

    // Quits the application
    public void QuitApplication()
    {
        GameLogger.Log("[GameManager.QuitApplication]: Quitting application.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Debug / Logging

    // Allows runtime toggling of global logging (e.g. from UI)
    public void SetLoggingEnabled(bool isEnabled)
    {
        enableLogging = isEnabled;
        GameLogger.IsEnabled = isEnabled;
    }

    #endregion
}
