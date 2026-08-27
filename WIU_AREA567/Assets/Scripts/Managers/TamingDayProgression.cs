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

        int amount =
            dayManager.DaysRemaining > 1
                ? 2
                : 1;

        dayManager.AdvanceDays(amount);
    }
}