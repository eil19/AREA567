using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string menuScene = "MenuScene";
    [SerializeField] private string introCutscene = "IntroCutsceneScene";
    [SerializeField] private string presentScene = "PresentScene";
    [SerializeField] private string pastScene = "PastScene";
    [SerializeField] private string bossScene = "BossScene";

    [Header("References")]
    [SerializeField] private GameSessionManager gameSessionManager;
    [SerializeField] private AsyncLoader asyncLoader;

    public void StartNewGame()
    {
        gameSessionManager?.ResetRun();

        LoadScene(introCutscene);
    }

    public void FinishIntroCutscene()
    {
        LoadScene(presentScene);
    }

    public void LoadPresent()
    {
        LoadScene(presentScene);
    }

    public void LoadPast()
    {
        LoadScene(pastScene);
    }

    public void LoadBoss()
    {
        LoadScene(bossScene);
    }

    public void ReplayGame()
    {
        gameSessionManager?.ResetRun();

        LoadScene(introCutscene);
    }

    public void ReturnToMainMenu()
    {
        gameSessionManager?.ResetRun();

        LoadScene(menuScene);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return;

        if (asyncLoader != null)
        {
            asyncLoader.LoadScene(sceneName);
        }
        else
        {
            // Safe fallback.
            SceneManager.LoadScene(sceneName);
        }
    }
}