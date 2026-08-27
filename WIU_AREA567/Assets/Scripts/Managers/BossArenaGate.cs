using UnityEngine;
using UnityEngine.Events;

public class BossArenaGate : MonoBehaviour,
    IInteractable
{
    [Header("References")]
    [SerializeField]
    private DayManager dayManager;

    [Header("Events")]
    public UnityEvent OnBossAreaUnlocked;

    public UnityEvent OnBossAreaLocked;

    public UnityEvent OnBossFightRequested;

    private bool isUnlocked;

    public bool IsUnlocked => isUnlocked;

    private void Start()
    {
        if (dayManager == null)
        {
            dayManager =
                FindFirstObjectByType<DayManager>();
        }

        if (dayManager == null)
        {
            Debug.LogWarning(
                "BossArenaGate: DayManager not found."
            );

            return;
        }

        dayManager.OnDayChanged.AddListener(
            CheckBossAvailability
        );

        CheckBossAvailability(
            dayManager.DaysRemaining
        );
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged.RemoveListener(
                CheckBossAvailability
            );
        }
    }

    private void CheckBossAvailability(
        int daysRemaining)
    {
        bool shouldUnlock =
            daysRemaining <= 0;

        if (isUnlocked == shouldUnlock)
        {
            return;
        }

        isUnlocked = shouldUnlock;

        if (isUnlocked)
        {
            Debug.Log(
                "Boss area is now available."
            );

            OnBossAreaUnlocked?.Invoke();
        }
    }

    public void Interact(
        GameObject interactor)
    {
        if (!isUnlocked)
        {
            Debug.Log(
                "The Alien Queen has not returned yet."
            );

            OnBossAreaLocked?.Invoke();
            return;
        }

        // Do NOT load BossScene immediately.
        // Ask the player first.
        OnBossFightRequested?.Invoke();
    }
}