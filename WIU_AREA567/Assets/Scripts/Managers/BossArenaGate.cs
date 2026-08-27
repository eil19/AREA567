using UnityEngine;
using UnityEngine.Events;

public class BossAreaGate : MonoBehaviour,
    IInteractable
{
    [Header("References")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private SceneFlowManager sceneFlowManager;

    [Header("Events")]
    public UnityEvent OnBossAreaUnlocked;
    public UnityEvent OnBossAreaLocked;

    private bool isUnlocked;

    private void Start()
    {
        if (dayManager == null)
        {
            dayManager =
                FindFirstObjectByType<DayManager>();
        }

        if (sceneFlowManager == null)
        {
            sceneFlowManager =
                FindFirstObjectByType<SceneFlowManager>();
        }

        if (dayManager != null)
        {
            dayManager.OnDayChanged.AddListener(
                CheckBossAvailability
            );

            CheckBossAvailability(
                dayManager.DaysRemaining
            );
        }
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

        if (shouldUnlock &&
            !isUnlocked)
        {
            isUnlocked = true;

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

        if (sceneFlowManager != null)
        {
            sceneFlowManager.LoadBoss();
        }
    }
}