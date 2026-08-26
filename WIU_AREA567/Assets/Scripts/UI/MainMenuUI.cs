using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject howToPlay;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject credits;

    [Header("Overlay")]
    [SerializeField] private GameObject overlay;

    [Header("Intro")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private IntroCutsceneController introCutsceneController;

    [Header("References")]
    [SerializeField] private GameSessionManager gameSessionManager;
    [SerializeField] private AsyncLoader asyncLoader;

    private void Start()
    {
        ShowMainMenu();
        if (introPanel != null)
        {
            introPanel.SetActive(false);
        }
    }

    public void StartNewGame()
    {
        if (gameSessionManager == null)
        {
            gameSessionManager =
                FindFirstObjectByType<GameSessionManager>();
        }

        gameSessionManager?.ResetRun();

        HideMainMenu();

        if (introPanel != null)
        {
            introPanel.SetActive(true);
        }

        if (introCutsceneController != null)
        {
            introCutsceneController.BeginIntro();
        }
    }
    public void NotReady()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(false);
        }

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        overlay.SetActive(false);
        CloseAllPopups();
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
        overlay.SetActive(false);
        CloseAllPopups();
    }

    public void OpenControls()
    {
        OpenPopup(controls);
    }

    public void OpenHowToPlay()
    {
        OpenPopup(howToPlay);
    }

    public void OpenSettings()
    {
        OpenPopup(settings);
    }

    public void OpenCredits()
    {
        OpenPopup(credits);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    private void OpenPopup(GameObject popup)
    {
        CloseAllPopups();
        overlay.SetActive(true);
        popup.SetActive(true);
    }

    private void CloseAllPopups()
    {
        controls.SetActive(false);
        howToPlay.SetActive(false);
        settings.SetActive(false);
        credits.SetActive(false);
    }
}