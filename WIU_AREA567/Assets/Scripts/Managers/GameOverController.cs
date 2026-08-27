using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverPanel;

    private GameSessionManager session;
    private Damageable playerHealth;
    private PlayerInputLock inputLock;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        session =
            FindFirstObjectByType<
                GameSessionManager>();

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (player != null)
        {
            playerHealth =
                player.GetComponent<Damageable>();

            inputLock =
                player.GetComponent<
                    PlayerInputLock>();

            if (playerHealth != null)
            {
                playerHealth.OnDeath
                    .AddListener(ShowGameOver);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath
                .RemoveListener(ShowGameOver);
        }
    }

    public void ShowGameOver()
    {
        inputLock?.LockAll();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1f;

        session?.ResetRun();

        SceneManager.LoadScene(
            "MenuScene"
        );
    }
}