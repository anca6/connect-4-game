using UnityEngine;

// ClickHandler:
// - Attached to column click colliders
// - Holds column index and forwards clicks to GameController
public class ClickHandler : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    private int _columnIndex;

    public void SetColumnIndex(int index) => _columnIndex = index;
    public void SetGameController(GameController controller) => gameController = controller;

    // Called from GameController when raycast hits this object
    public void HandleClick()
    {
        if (gameController == null)
        {
            GameLogger.LogError("[ClickHandler.HandleClick]: GameController not assigned.");
            return;
        }

        GameLogger.Log($"[ClickHandler.HandleClick]: Column {_columnIndex}");
        gameController.PlayInColumn(_columnIndex);
    }
}
