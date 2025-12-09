using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private int columnIndex;
    [SerializeField] private GameController gameController;

    public void SetColumnIndex(int index) => columnIndex = index;
    public void SetGameController(GameController controller) => gameController = controller;

    // Called from GameController when raycast hits this object
    public void HandleClick()
    {
        if (gameController == null)
        {
            Debug.LogError("GameController not assigned on ClickHandler.", this);
            return;
        }

        Debug.Log($"ClickHandler.HandleClick: column {columnIndex}", this);
        gameController.TryPlayInColumn(columnIndex);
    }
}
