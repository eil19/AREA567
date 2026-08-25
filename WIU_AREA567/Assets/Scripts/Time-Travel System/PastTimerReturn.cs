using UnityEngine;

public class PastTimerReturn : MonoBehaviour
{
    [SerializeField] private TimeTravelController timeTravelController;

    private void Start()
    {
        if (timeTravelController == null)
        {
            timeTravelController = FindFirstObjectByType<TimeTravelController>();
        }
    }

    public void ReturnToPresent()
    {
        if (timeTravelController == null) return;
        timeTravelController.TravelToPresent();
    }
}