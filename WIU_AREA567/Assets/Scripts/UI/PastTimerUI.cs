using TMPro;
using UnityEngine;

public class PastTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    public void UpdateTimer(float remainingTime)
    {
        if (timerText == null) return;

        int totalSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = minutes.ToString("00") +
            ":" + seconds.ToString("00");
    }
}