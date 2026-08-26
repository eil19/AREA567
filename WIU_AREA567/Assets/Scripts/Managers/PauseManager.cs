using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject overlay;

    [Header("References")]
    [SerializeField]
    private GameSessionManager gameSessionManager;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        if (overlay != null)
            overlay.SetActive(false);

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (!InputSystem.actions["Pause"].WasPressedThisFrame())
        {
            return;
        }

        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;

        if (overlay != null)
            overlay.SetActive(true);

        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;

        if (overlay != null)
            overlay.SetActive(false);

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenSetting()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        if (gameSessionManager == null)
        {
            gameSessionManager =
                FindFirstObjectByType<
                    GameSessionManager>();
        }

        gameSessionManager?.ResetRun();

        SceneManager.LoadScene(
            "MenuScene"
        );
    }
}