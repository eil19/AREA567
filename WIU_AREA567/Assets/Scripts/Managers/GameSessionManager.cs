using UnityEngine;
using UnityEngine.Events;

public class GameSessionManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ResearchLog researchLog;
    [SerializeField] private ChestStorageService chestStorage;
    [SerializeField] private DayManager dayManager;
    [SerializeField] private TimeTravelController timeTravelController;

    [Header("Events")]
    public UnityEvent OnRunReset;

    public void ResetRun()
    {
        OrganisationRunData.ResetRun();
        DoorRunData.ResetRun();

        if (inventory != null)
            inventory.ClearInventory();

        if (researchLog != null)
            researchLog.ClearResearch();

        if (chestStorage != null)
            chestStorage.ClearAll();

        if (dayManager != null)
        {
            dayManager.SetDaysRemaining(
                dayManager.MaximumDays
            );
        }

        if (timeTravelController != null)
            timeTravelController.ResetRun();

        OnRunReset?.Invoke();

        Debug.Log("Game reset.");
    }
}