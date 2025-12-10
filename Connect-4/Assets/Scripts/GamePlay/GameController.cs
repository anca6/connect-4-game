using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// GameController:
// - Handles gameplay on the GameScene
// - Connects BoardState, WinChecker, BoardView, and UI
// - Handles player input, turn flow, win/draw detection, and undo
public class GameController : MonoBehaviour
{
    #region Fields

    [Header("View")]
    [SerializeField] private BoardView boardView;
    [SerializeField] private BoardHighlighter boardHighlighter;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    private GameManager _gameManager;
    private bool _isAnimatingMove = false;

    #endregion

    #region Properties

    private GameConfigurator Config => _gameManager.Config;
    private BoardState BoardState => _gameManager.BoardState;

    private int CurrentPlayerId
    {
        get => _gameManager.CurrentPlayerId;
        set => _gameManager.CurrentPlayerId = value;
    }

    private bool IsGameOver
    {
        get => _gameManager.IsGameOver;
        set => _gameManager.IsGameOver = value;
    }

    private Stack<MoveResult> MoveHistory => _gameManager.MoveHistory;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        _gameManager = GameManager.Instance;
        if (_gameManager == null)
        {
            GameLogger.LogError("[GameController.Start]: No GameManager instance found in scene.");
            enabled = false;
            return;
        }

        if (Config == null)
        {
            GameLogger.LogError("[GameController.Start]: GameConfigurator not assigned on GameManager.");
            enabled = false;
            return;
        }

        if (boardView == null)
        {
            GameLogger.LogError("[GameController.Start]: BoardView not assigned.");
            enabled = false;
            return;
        }

        if (boardHighlighter == null)
        {
            GameLogger.LogWarning("[GameController.Start]: BoardHighlighter not assigned.");
        }

        // creating a new game if none exists yet
        if (!_gameManager.HasActiveGame)
        {
            _gameManager.StartNewGame();
        }

        // building visuals from existing logical state
        RebuildBoardViewFromState();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        // ignoring input if pointer is over a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        HandleClick();
    }

    #endregion

    #region Input Handling

    // Handles the click on the board using a raycast and ClickHandler
    private void HandleClick()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameLogger.LogError("[GameController.HandleClick]: No MainCamera in scene.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            GameLogger.Log($"[GameController.HandleClick]: Raycast hit {hit.collider.gameObject.name}");

            ClickHandler handler = hit.collider.GetComponent<ClickHandler>();
            if (handler != null)
            {
                handler.HandleClick();
            }
        }
        else
        {
            GameLogger.Log("[GameController.HandleClick]: Raycast hit nothing.");
        }
    }

    #endregion

    #region Game Flow

    // Rebuilds the visual board from the current BoardState in GameManager (used on scene load / Load Game)
    private void RebuildBoardViewFromState()
    {
        _isAnimatingMove = false;

        if (boardHighlighter != null)
        {
            boardHighlighter.ClearHighlights();
        }

        boardView.ClearDiscs();
        boardView.Initialize(Config.rows, Config.columns);

        if (BoardState == null)
        {
            CurrentPlayerId = 1;
            IsGameOver = false;
            UpdateStatusText($"Player {CurrentPlayerId}'s turn");
            return;
        }

        // re-creating discs from BoardState (no animation for existing state)
        for (int row = 0; row < BoardState.Rows; row++)
        {
            for (int col = 0; col < BoardState.Columns; col++)
            {
                int playerId = BoardState.GetCell(row, col);
                if (playerId != 0)
                {
                    var pos = new BoardPosition(row, col);
                    boardView.SpawnDiscImmediate(pos, playerId);
                }
            }
        }

        if (IsGameOver)
        {
            UpdateStatusText("Game over");
        }
        else
        {
            UpdateStatusText($"Player {CurrentPlayerId}'s turn");
        }
    }

    // Starts a completely new game
    public void StartNewGame()
    {
        _gameManager.StartNewGame();
        RebuildBoardViewFromState();
    }


    // Called when a player attempts to play in a column
    public void PlayInColumn(int column)
    {
        GameLogger.Log($"[GameController.PlayInColumn]: Column {column}");

        if (IsGameOver || _isAnimatingMove)
            return;

        if (!BoardState.CanPlay(column))
        {
            GameLogger.Log("[GameController.PlayInColumn]: Column is full.");
            return;
        }

        MoveResult move = BoardState.PlayMove(column, CurrentPlayerId);

        if (!move.Success)
            return;

        // storing the move for undo in shared state
        MoveHistory.Push(move);

        GameLogger.Log(
            $"[GameController.PlayInColumn]: Spawning disc at {move.Position} for player {move.PlayerId}");

        // determining win / draw on the updated board
        WinResult winResult = WinChecker.CheckForWin(
            BoardState,
            move.Position,
            move.PlayerId,
            Config.connectLength);

        bool isDraw = BoardState.IsFull();

        _isAnimatingMove = true;

        // visual spawn with callback when drop finishes
        boardView.SpawnDisc(move.Position, move.PlayerId, () =>
        {
            _isAnimatingMove = false;

            // win
            if (winResult.IsWin)
            {
                IsGameOver = true;

                if (boardHighlighter != null && winResult.WinningPositions != null)
                {
                    boardHighlighter.HighlightWinningLine(winResult.WinningPositions);
                }

                UpdateStatusText($"Player {CurrentPlayerId} wins!");
                return;
            }

            // draw
            if (isDraw)
            {
                IsGameOver = true;
                UpdateStatusText("Draw!");
                return;
            }

            // normal turn switch
            CurrentPlayerId++;
            if (CurrentPlayerId > Config.playerCount)
                CurrentPlayerId = 1;

            UpdateStatusText($"Player {CurrentPlayerId}'s turn");
        });
    }

    // Public undo method
    public void UndoLastMove()
    {
        if (_isAnimatingMove)
            return;

        if (MoveHistory.Count == 0)
        {
            GameLogger.Log("[GameController.UndoLastMove]: No moves to undo.");
            return;
        }

        if (boardHighlighter != null)
        {
            boardHighlighter.ClearHighlights();
        }

        MoveResult lastMove = MoveHistory.Pop();

        // undoing in model
        BoardState.UndoMove(lastMove.Position);

        // undoing in view
        boardView.RemoveDisc(lastMove.Position);

        IsGameOver = false;

        // setting turn back to the player who made the undone move
        CurrentPlayerId = lastMove.PlayerId;
        UpdateStatusText($"Player {CurrentPlayerId}'s turn");
    }

    #endregion

    #region UI Helpers

    // Updates the status text label
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        else
        {
            GameLogger.Log($"[GameController.UpdateStatusText]: {message}");
        }
    }

    #endregion
}
