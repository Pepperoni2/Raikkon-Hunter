using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WinScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag Win Screen UI Panel here")]
    [SerializeField] private GameObject winScreenUI;

    [Header("Player Reference")]
    [Tooltip("Drag Player GameObject here to disable their controls")]
    [SerializeField] private PlayerInput playerInput;

    private void Start()
    {
        if (winScreenUI != null)
        {
            winScreenUI.SetActive(false);
        }
    }

    public void ShowWinScreen()
    {
        winScreenUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game physics and animations

        if (playerInput != null)
        {
            playerInput.DeactivateInput(); 
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game from Win Screen...");
        Application.Quit();
    }
}