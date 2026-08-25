using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ResearchLog researchLog;
    [SerializeField] private ChestStorageService chestStorage;
    [SerializeField] private DayManager dayManager;
    [SerializeField] private TimeTravelController timeTravelController;

    public void ResetRun()
    {
        if (inventory != null)
        {
            inventory.ClearInventory();
        }

        if (researchLog != null)
        {
            researchLog.ClearResearch();
        }

        if (chestStorage != null)
        {
            chestStorage.ClearAll();
        }

        if (dayManager != null)
        {
            dayManager.SetDaysRemaining(dayManager.MaximumDays);
        }

        if (timeTravelController != null)
        {
            timeTravelController.ResetRun();
        }

        Debug.Log("Game reset.");
    }
}