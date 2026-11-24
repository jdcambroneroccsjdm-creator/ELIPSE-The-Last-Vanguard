
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Death Screen")]
    public GameObject deathPanel;

    [Header("Buttons")]
    public Button respawnButton;
    public Button mainMenuButton;

    [Header("Player")]
    public PlayerHealth playerHealth;

    [Header("Main Menu")]
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        Time.timeScale = 1f;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (respawnButton != null)
            respawnButton.onClick.AddListener(RespawnPlayer);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void ShowDeathScreen()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Pause the game while the death screen is open.
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideDeathScreen()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void RespawnPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.Respawn();
            HideDeathScreen();
        }
        else
        {
            Debug.LogError("Assign PlayerHealth in GameOverUI.");
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError("Enter your main menu scene name.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
