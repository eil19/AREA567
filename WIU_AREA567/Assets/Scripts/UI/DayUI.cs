using TMPro;
using UnityEngine;

public class DayUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text daysRemainingText;

    private DayManager dayManager;

    private void Start()
    {
        dayManager =
            FindFirstObjectByType<DayManager>();

        if (dayManager == null)
            return;

        dayManager.OnDayChanged.AddListener(
            UpdateUI
        );

        UpdateUI(
            dayManager.DaysRemaining
        );
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged
                .RemoveListener(UpdateUI);
        }
    }

    private void UpdateUI(int days)
    {
        if (daysRemainingText == null)
            return;

        if (days <= 0)
        {
            daysRemainingText.text =
                "ALIEN QUEEN\nHAS RETURNED";
        }
        else if (days == 1)
        {
            daysRemainingText.text =
                "QUEEN ARRIVAL IN\n1 DAY";
        }
        else
        {
            daysRemainingText.text =
                "QUEEN ARRIVAL IN\n" +
                days +
                " DAYS";
        }
    }
}