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

    private void Start()
    {
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