using UnityEngine;

public class TamingDayProgression : MonoBehaviour
{
    [SerializeField]
    private DayManager dayManager;

    private void Start()
    {
        if (dayManager == null)
        {
            dayManager =
                FindFirstObjectByType<DayManager>();
        }
    }

    public void RegisterSuccessfulTame()
    {
        if (dayManager == null)
            return;

        if (dayManager.DaysRemaining <= 0)
            return;

        int daysRemaining =
            dayManager.DaysRemaining;

        int amount;

        // Three tame milestones:
        //
        // 7 -> 5
        // 5 -> 3
        // 3 -> 0
        if (daysRemaining <= 3)
        {
            amount = daysRemaining;
        }
        else
        {
            amount = 2;
        }

        dayManager.AdvanceDays(amount);
    }
}