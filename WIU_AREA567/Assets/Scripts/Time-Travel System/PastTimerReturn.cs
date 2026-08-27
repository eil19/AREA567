using UnityEngine;
using UnityEngine.SceneManagement;

public class PastTimerReturn : MonoBehaviour
{
    [SerializeField]
    private TimeTravelController
        timeTravelController;

    private void Start()
    {
        FindController();
    }

    private void FindController()
    {
        if (timeTravelController == null)
        {
            timeTravelController =
                FindFirstObjectByType<
                    TimeTravelController>();
        }
    }

    public void ReturnToPresent()
    {
        if (timeTravelController == null)
        {
            FindController();
        }

        if (timeTravelController != null)
        {
            timeTravelController
                .TravelToPresent();

            return;
        }

        // Emergency fallback.
        // This should only happen if the
        // persistent manager is somehow missing.
        Debug.LogWarning(
            "TimeTravelController missing. " +
            "Loading PresentScene directly."
        );

        SceneManager.LoadScene(
            "PresentScene"
        );
    }
}