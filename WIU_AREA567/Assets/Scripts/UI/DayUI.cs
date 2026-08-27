using TMPro;
using UnityEngine;

public class DayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text daysRemainingText;

    private DayManager dayManager;

    private void Start()
    {
        dayManager = FindFirstObjectByType<DayManager>();

        if (dayManager == null)
        {
            return;
        }

        dayManager.OnDayChanged.AddListener(
            UpdateDayUI
        );

        UpdateDayUI(
            dayManager.DaysRemaining
        );
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged.RemoveListener(
                UpdateDayUI
            );
        }
    }

    private void UpdateDayUI(
        int daysRemaining)
    {
        if (daysRemainingText == null)
        {
            return;
        }

        if (daysRemaining <= 0)
        {
            daysRemainingText.text =
                "ALIEN QUEEN\nHAS RETURNED";

            return;
        }

        if (daysRemaining == 1)
        {
            daysRemainingText.text =
                "QUEEN ARRIVAL IN\n1 DAY";

            return;
        }

        daysRemainingText.text =
            "QUEEN ARRIVAL IN\n" +
            daysRemaining +
            " DAYS";
    }
}