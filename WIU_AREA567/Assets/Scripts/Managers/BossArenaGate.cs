using UnityEngine;
using UnityEngine.Events;

public class BossArenaGate :
    MonoBehaviour,
    IInteractable
{
    [SerializeField]
    private DayManager dayManager;

    [Header("Events")]
    public UnityEvent OnBossAreaLocked;
    public UnityEvent OnBossFightRequested;

    private bool isUnlocked;

    private void Start()
    {
        if (dayManager == null)
        {
            dayManager =
                FindFirstObjectByType<DayManager>();
        }

        if (dayManager != null)
        {
            dayManager.OnDayChanged.AddListener(
                CheckAvailability
            );

            CheckAvailability(
                dayManager.DaysRemaining
            );
        }
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged
                .RemoveListener(
                    CheckAvailability
                );
        }
    }

    private void CheckAvailability(int days)
    {
        isUnlocked = days <= 0;
    }

    public void Interact(GameObject interactor)
    {
        if (!isUnlocked)
        {
            OnBossAreaLocked?.Invoke();
            return;
        }

        OnBossFightRequested?.Invoke();
    }
}