
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseGame : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause Panel is not assigned!");
        }

        Time.timeScale = 1f;
    }

    void Update()
    {
        bool pausePressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pausePressed = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePressed = true;
        }
#endif

        if (pausePressed)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGameMenu();
        }
    }

    public void PauseGameMenu()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Debug.Log("Game Resumed");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
