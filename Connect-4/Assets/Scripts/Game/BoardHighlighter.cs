using System.Collections.Generic;
using UnityEngine;

public class BoardHighlighter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardView boardView;

    [Header("Highlight Settings")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float scaleMultiplier = 1.2f;

    // Track original state so we can revert
    private readonly Dictionary<BoardPosition, Vector3> _originalScales = new();
    private readonly Dictionary<BoardPosition, Material> _originalMaterials = new();

    // Called by GameController when someone wins
    public void HighlightWinningLine(IReadOnlyList<BoardPosition> winningPositions)
    {
        if (winningPositions == null || winningPositions.Count == 0)
            return;

        foreach (BoardPosition pos in winningPositions)
        {
            ApplyHighlight(pos);
        }
    }

    // Called by GameController on undo / new game
    public void ClearHighlights()
    {
        // Restore scales
        foreach (var kvp in _originalScales)
        {
            BoardPosition pos = kvp.Key;
            Vector3 originalScale = kvp.Value;

            if (boardView.GetDisc(pos, out GameObject disc) && disc != null)
            {
                disc.transform.localScale = originalScale;
            }
        }

        // Restore materials
        foreach (var kvp in _originalMaterials)
        {
            BoardPosition pos = kvp.Key;
            Material originalMat = kvp.Value;

            if (boardView.GetDisc(pos, out GameObject disc) && disc != null)
            {
                Renderer renderer = disc.GetComponentInChildren<Renderer>();
                if (renderer != null && originalMat != null)
                {
                    renderer.material = originalMat;
                }
            }
        }

        _originalScales.Clear();
        _originalMaterials.Clear();
    }

    private void ApplyHighlight(BoardPosition pos)
    {
        if (!boardView.GetDisc(pos, out GameObject disc) || disc == null)
            return;

        // Cache original scale once
        if (!_originalScales.ContainsKey(pos))
        {
            _originalScales[pos] = disc.transform.localScale;
        }

        // Cache original material once
        Renderer renderer = disc.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            if (!_originalMaterials.ContainsKey(pos))
            {
                _originalMaterials[pos] = renderer.material;
            }

            if (highlightMaterial != null)
            {
                renderer.material = highlightMaterial;
            }
        }

        // Simple scale-up highlight
        disc.transform.localScale = _originalScales[pos] * scaleMultiplier;
    }
}
