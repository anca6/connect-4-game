using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameConfigurator config;

    [Header("View")]
    [SerializeField] private BoardView boardView;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    private BoardState _boardState;
    private int _currentPlayerId = 1;
    private bool _isGameOver = false;

    private Stack<MoveResult> _moveHistory = new Stack<MoveResult>();

    private void Start()
    {
        if (config == null)
        {
            Debug.LogWarning("GameConfig not assigned.");
            enabled = false;
            return;
        }

        if (boardView == null)
        {
            Debug.LogError("BoardView not assigned.");
            enabled = false;
            return;
        }

        StartNewGame();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No MainCamera in scene!");
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");

            ClickHandler handler = hit.collider.GetComponent<ClickHandler>();
            if (handler != null)
            {
                handler.HandleClick();
            }
        }
        else
        {
            Debug.Log("Raycast hit nothing");
        }
    }

    public void StartNewGame()
    {
        _boardState = new BoardState(config.rows, config.columns);
        _currentPlayerId = 1;
        _isGameOver = false;

        _moveHistory.Clear();

        // Clear old discs from view
        boardView.ClearDiscs();

        boardView.Initialize(config.rows, config.columns);
        UpdateStatusText($"Player {_currentPlayerId}'s turn");
    }

    // Called when a player attempts to play in a column
    public void TryPlayInColumn(int column)
    {
        Debug.Log($"GameController.TryPlayInColumn({column})");

        if (_isGameOver)
            return;

        if (!_boardState.CanPlay(column))
        {
            Debug.Log("Column is full.");
            return;
        }

        MoveResult move = _boardState.PlayMove(column, _currentPlayerId);

        if (!move.Success)
            return;

        Debug.Log($"Spawning disc at {move.Position} for player {move.PlayerId}");

        // Visual spawn
        boardView.SpawnDisc(move.Position, move.PlayerId);

        _moveHistory.Push(new MoveResult(true, move.PlayerId, move.Position));

        // Win check
        WinResult winResult = WinChecker.CheckForWin(
            _boardState,
            move.Position,
            move.PlayerId,
            config.connectLength);

        if (winResult.IsWin)
        {
            _isGameOver = true;
            UpdateStatusText($"Player {_currentPlayerId} wins!");
            return;
        }

        // Draw check
        if (_boardState.IsFull())
        {
            _isGameOver = true;
            UpdateStatusText("Draw!");
            return;
        }

        // Switch player
        _currentPlayerId++;
        if(_currentPlayerId > config.playerCount) _currentPlayerId = 1;

        UpdateStatusText($"Player {_currentPlayerId}'s turn");
    }

    // public undo method (hook this to a UI button)
    public void UndoLastMove()
    {
        if (_moveHistory.Count == 0)
        {
            Debug.Log("No moves to undo.");
            return;
        }

        MoveResult lastMove = _moveHistory.Pop();

        // Undo in model
        _boardState.UndoMove(lastMove.Position);

        // Undo in view
        boardView.RemoveDisc(lastMove.Position);

        // Once we undo, the game is definitely not over from that last move
        _isGameOver = false;

        // Set turn back to the player who made the undone move
        _currentPlayerId = lastMove.PlayerId;
        UpdateStatusText($"Player {_currentPlayerId}'s turn");
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        else
        {
            Debug.Log(message);
        }
    }
}
