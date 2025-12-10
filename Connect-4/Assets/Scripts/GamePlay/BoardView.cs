using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BoardView:
// - Handles the visual representation of the board
// - Spawns cells and column click areas
// - Spawns, animates, and removes discs for each move
public class BoardView : MonoBehaviour
{
    #region Fields

    [Header("Layout")]
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Transform tableTransform;
    [SerializeField] private float boardHeightOffset = 0f;

    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject columnClickPrefab;

    [Header("Visuals")]
    [SerializeField] private GameObject discPrefab;
    [SerializeField] private Transform discsParent;
    [SerializeField] private Material[] playerMaterials;

    [Header("Disc Animation")]
    [SerializeField] private float dropHeight = 20f;
    [SerializeField] private float dropDuration = 0.8f;
    [SerializeField] private float collisionDetectionRadius = 1f;
    [SerializeField] private LayerMask cellLayerMask;
    [SerializeField] private float spinSpeed = 540f;

    private int _rows;
    private int _columns;

    private GameObject[,] _cellInstances;
    private readonly Dictionary<BoardPosition, GameObject> _discInstances = new();

    private Vector3 _origin = Vector3.zero;
    private Vector3 _boardOrigin;

    #endregion

    #region Initialization

    // Initializes the board view with the given dimensions
    public void Initialize(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;

        _cellInstances = new GameObject[_rows, _columns];

        // computing bottom left corner so the board is centered on the table
        Vector3 center;

        if (tableTransform != null)
        {
            center = tableTransform.position;
        }
        else
        {
            center = _origin;
            GameLogger.LogWarning("[BoardView.Initialize]: Table Transform not assigned.");
        }

        float width = _columns * cellSize;
        float boardBottomY = center.y + boardHeightOffset;

        _boardOrigin = new Vector3(
            center.x - width * 0.5f,
            boardBottomY,
            center.z);

        GenerateCells();
        GenerateColumnClickAreas();
    }

    // Generates the visual cells for the board grid
    private void GenerateCells()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Vector3 pos = GetWorldPosition(row, col);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                _cellInstances[row, col] = cell;
            }
        }
    }

    // Generates clickable areas for each column and wires them to GameController
    private void GenerateColumnClickAreas()
    {
        GameController controller = FindAnyObjectByType<GameController>();

        float half = cellSize * 0.5f;

        for (int col = 0; col < _columns; col++)
        {
            float x = _boardOrigin.x + col * cellSize + half;
            float yCenter = _boardOrigin.y + _rows * cellSize * 0.5f;

            Vector3 pos = new Vector3(x, yCenter, _boardOrigin.z);

            GameObject clickObj = Instantiate(columnClickPrefab, pos, Quaternion.identity, transform);

            // scaling so it covers the whole column vertically
            Vector3 scale = clickObj.transform.localScale;
            scale.y = _rows * cellSize;
            clickObj.transform.localScale = scale;

            ClickHandler handler = clickObj.GetComponent<ClickHandler>();
            if (handler != null)
            {
                handler.SetColumnIndex(col);
                handler.SetGameController(controller);
            }
        }
    }

    #endregion

    #region Disc Spawning

    // Animated spawn for a newly played move
    public void SpawnDisc(BoardPosition pos, int playerId, Action onDropComplete = null)
    {
        Vector3 targetPos = GetWorldPosition(pos.Row, pos.Column);
        Vector3 spawnPos = targetPos + Vector3.up * dropHeight;

        GameObject disc = Instantiate(discPrefab, spawnPos, Quaternion.identity, discsParent);

        if (playerId > 0 && playerId <= playerMaterials.Length)
        {
            Renderer renderer = disc.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material = playerMaterials[playerId - 1];
            }
        }

        Rigidbody rb = disc.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // tracking this disc for undo / highlighting
        _discInstances[pos] = disc;

        StartCoroutine(AnimateDiscDrop(disc.transform, targetPos, onDropComplete));
    }

    // Instant spawn for reconstructing from an existing BoardState (Load Game, scene reload)
    public void SpawnDiscImmediate(BoardPosition pos, int playerId)
    {
        Vector3 targetPos = GetWorldPosition(pos.Row, pos.Column);

        GameObject disc = Instantiate(discPrefab, targetPos, Quaternion.identity, discsParent);

        if (playerId > 0 && playerId <= playerMaterials.Length)
        {
            Renderer renderer = disc.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material = playerMaterials[playerId - 1];
            }
        }

        Rigidbody rb = disc.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        _discInstances[pos] = disc;
    }

    #endregion

    #region Disc Animation

    // Animates the disc dropping to its target position
    private IEnumerator AnimateDiscDrop(Transform disc, Vector3 targetPos, Action onComplete)
    {
        Vector3 startPos = disc.position;
        float t = 0f;

        bool spinning = true;

        while (t < 1f)
        {
            t += Time.deltaTime / dropDuration;

            if (spinning)
            {
                // rotating while falling
                disc.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);

                // checking for collision with board cells to stop spinning
                Collider[] hits = Physics.OverlapSphere(
                    disc.position,
                    collisionDetectionRadius,
                    cellLayerMask,
                    QueryTriggerInteraction.Collide);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform == disc) continue;

                    spinning = false;
                    disc.rotation = Quaternion.identity;
                    break;
                }
            }

            disc.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        disc.position = targetPos;
        disc.rotation = Quaternion.identity;

        onComplete?.Invoke();
    }

    #endregion

    #region Disc Management

    // Removes a single disc at a position
    public void RemoveDisc(BoardPosition pos)
    {
        if (_discInstances.TryGetValue(pos, out GameObject disc) && disc != null)
        {
            Destroy(disc);
        }

        _discInstances.Remove(pos);
    }

    // Clears all discs (for restarting the game)
    public void ClearDiscs()
    {
        foreach (var kvp in _discInstances)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }

        _discInstances.Clear();
    }

    // Gets the disc game object at a given board position
    public bool GetDisc(BoardPosition pos, out GameObject disc)
    {
        return _discInstances.TryGetValue(pos, out disc);
    }

    #endregion

    #region Helpers

    // Converts board coordinates (row, col) to a world position
    private Vector3 GetWorldPosition(int row, int col)
    {
        float half = cellSize * 0.5f;

        float x = _boardOrigin.x + col * cellSize + half;
        float y = _boardOrigin.y + row * cellSize + half;
        float z = _boardOrigin.z;

        return new Vector3(x, y, z);
    }

    #endregion
}
