using UnityEngine;
using UnityEngine.SceneManagement;

// UIController:
// - Handles main menu and in game UI button clicks
// - Delegates to GameManager for starting/loading games and switching scenes
public class UIController : MonoBehaviour
{
    [SerializeField] private string fallbackStartSceneName = "StartScene";

    // ===== START MENU BUTTONS =====

    public void OnNewGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGameAndLoad();
        }
        else
        {
            GameLogger.LogError("[UIController.OnNewGameClicked]: GameManager instance not found.");
        }
    }

    public void OnLoadGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();
        }
        else
        {
            GameLogger.LogError("[UIController.OnLoadGameClicked]: GameManager instance not found.");
        }
    }

    public void OnQuitApplicationClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ===== IN-GAME BUTTONS =====

    public void OnQuitToMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            SceneManager.LoadScene(fallbackStartSceneName);
        }
    }

    public void OnRestartGameClicked()
    {
        if (GameManager.Instance != null)
        {
            // Fresh game state
            GameManager.Instance.StartNewGame();
        }
        else
        {
            GameLogger.LogWarning("[UIController.OnRestartGameClicked]: No GameManager instance found when trying to restart game.");
        }
    }
}
