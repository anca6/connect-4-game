using System.Collections.Generic;
using UnityEngine;

// BoardHighlighter:
// - Highlights the winning discs when a player wins
// - Temporarily changes scale and material of discs
// - Restores original visuals on undo/new game
public class BoardHighlighter : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [SerializeField] private BoardView boardView;

    [Header("Highlight Settings")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float scaleMultiplier = 1f;

    // Tracking original state so we can revert
    private readonly Dictionary<BoardPosition, Vector3> _originalScales = new();
    private readonly Dictionary<BoardPosition, Material> _originalMaterials = new();

    #endregion

    #region Highlight

    // Called by GameController when someone wins
    public void HighlightWinningLine(IReadOnlyList<BoardPosition> winningPositions)
    {
        if (boardView == null)
        {
            GameLogger.LogWarning("[BoardHighlighter.HighlightWinningLine]: BoardView not assigned.");
            return;
        }

        if (winningPositions == null || winningPositions.Count == 0)
            return;

        foreach (BoardPosition pos in winningPositions)
        {
            ApplyHighlight(pos);
        }
    }

    // Called by GameController on undo/new game
    public void ClearHighlights()
    {
        if (boardView == null)
            return;

        // restoring scales
        foreach (var kvp in _originalScales)
        {
            BoardPosition pos = kvp.Key;
            Vector3 originalScale = kvp.Value;

            if (boardView.GetDisc(pos, out GameObject disc) && disc != null)
            {
                disc.transform.localScale = originalScale;
            }
        }

        // restoring materials
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

    #endregion

    #region Internals

    private void ApplyHighlight(BoardPosition pos)
    {
        if (!boardView.GetDisc(pos, out GameObject disc) || disc == null)
            return;

        // caching original scale once
        if (!_originalScales.ContainsKey(pos))
        {
            _originalScales[pos] = disc.transform.localScale;
        }

        // caching and overriding material
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

        // scale up highlight
        disc.transform.localScale = _originalScales[pos] * scaleMultiplier;
    }

    #endregion
}
