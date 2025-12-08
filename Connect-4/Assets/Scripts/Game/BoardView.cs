using UnityEngine;

public class BoardView : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 origin = Vector3.zero;

    [Header("Visuals")]
    [SerializeField] private GameObject discPrefab;  
    [SerializeField] private Transform discsParent; 
    [SerializeField] private Material[] playerMaterials; // each player has a different material for their discs

    private int _rows;
    private int _columns;

    public void Initialize(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
    }

    // Spawns a disc at the given board position
    public void SpawnDisc(BoardPosition pos, int playerId)
    {
        Vector3 worldPos = GetWorldPosition(pos.Row, pos.Column);

        GameObject disc = Instantiate(discPrefab, worldPos, Quaternion.identity, discsParent);

        // PLACEHOLDER
        // tint the disc based on playerId
        if (playerId > 0 && playerId <= playerMaterials.Length)
        {
            Renderer renderer = disc.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material = playerMaterials[playerId - 1];
            }
        }
    }

    // PLACEHOLDER
    // Converts board position to world position
    private Vector3 GetWorldPosition(int row, int col)
    {
        float x = origin.x + col * cellSize;
        float y = origin.y + row * cellSize;
        float z = origin.z;

        return new Vector3(x, y, z);
    }
}
