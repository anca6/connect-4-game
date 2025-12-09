using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 origin = Vector3.zero;

    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;     
    [SerializeField] private GameObject columnClickPrefab; 

    [Header("Visuals")]
    [SerializeField] private GameObject discPrefab;
    [SerializeField] private Transform discsParent;
    [SerializeField] private Material[] playerMaterials;

    [Header("Animation")]
    [SerializeField] private float dropHeight = 3f;
    [SerializeField] private float dropDuration = 0.25f;

    private int _rows;
    private int _columns;

    private GameObject[,] _cellInstances;

    public void Initialize(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;

        _cellInstances = new GameObject[_rows, _columns];

        GenerateCells();
        GenerateColumnClickAreas();
    }

    private void GenerateCells()
    {
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Vector3 pos = GetWorldPosition(row, col);
                GameObject cell = Object.Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                _cellInstances[row, col] = cell;
            }
        }
    }

    private void GenerateColumnClickAreas()
    {
        GameController controller = FindObjectOfType<GameController>();

        float half = cellSize * 0.5f;

        for (int col = 0; col < _columns; col++)
        {
            float x = origin.x + col * cellSize + half;

            float yCenter = origin.y + _rows * cellSize * 0.5f;

            Vector3 pos = new Vector3(x, yCenter, origin.z);

            GameObject clickObj = Instantiate(columnClickPrefab, pos, Quaternion.identity, transform);

            // scale so it covers the whole column vertically
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


    public void SpawnDisc(BoardPosition pos, int playerId)
    {
        Vector3 targetPos = GetWorldPosition(pos.Row, pos.Column);
        Vector3 spawnPos = targetPos + Vector3.up * dropHeight;

        GameObject disc = Object.Instantiate(discPrefab, spawnPos, Quaternion.identity, discsParent);

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

        StartCoroutine(AnimateDiscDrop(disc.transform, targetPos));
    }

    private IEnumerator AnimateDiscDrop(Transform disc, Vector3 targetPos)
    {
        Vector3 startPos = disc.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dropDuration;
            disc.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        disc.position = targetPos;
    }

    private Vector3 GetWorldPosition(int row, int col)
    {
        float half = cellSize * 0.5f;

        float x = origin.x + col * cellSize + half;
        float y = origin.y + row * cellSize + half;
        float z = origin.z;

        return new Vector3(x, y, z);
    }
}
