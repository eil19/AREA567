using UnityEngine;
using UnityEngine.Events;

public class DayManager : MonoBehaviour
{
    [Header("Day Settings")]
    [SerializeField] private int maximumDays = 7;
    [SerializeField] private int daysRemaining = 7;

    [Header("Events")]
    public UnityEvent<int> OnDayChanged;

    public int DaysRemaining => daysRemaining;
    public int MaximumDays => maximumDays;

    public void AdvanceDay()
    {
        if (daysRemaining <= 0)
            return;

        daysRemaining--;

        Debug.Log("Days remaining: " + daysRemaining);

        OnDayChanged?.Invoke(daysRemaining);
    }

    public void SetDaysRemaining(int days)
    {
        daysRemaining = Mathf.Clamp(days, 0, maximumDays);

        OnDayChanged?.Invoke(daysRemaining);
    }
}