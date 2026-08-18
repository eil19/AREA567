using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimelineVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private DayManager dayManager;
    [SerializeField] private TimeTravelController timeTravelController;

    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (globalVolume != null &&
            globalVolume.profile.TryGet(out ColorAdjustments adjustments))
        {
            colorAdjustments = adjustments;
        }
    }

    public void UpdateVisuals()
    {
        if (colorAdjustments == null) return;

        if (timeTravelController.CurrentTimeline == Timeline.Present)
        {
            colorAdjustments.saturation.value = 0f;
            return;
        }

        float dayProgress = 1f - ((float)dayManager.DaysRemaining / dayManager.MaximumDays);
        float saturation = Mathf.Lerp(-100f, -10f, dayProgress);
        colorAdjustments.saturation.value = saturation;
    }
}
