using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimelineVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private DayManager dayManager;
    [SerializeField] private TimeTravelController timeTravelController;

    [Header("Visual Settings")]
    [SerializeField] private float visualTransitionDuration = 0.4f;

    private ColorAdjustments colorAdjustments;
    private Coroutine saturationCoroutine;

    private void Awake()
    {
        if (globalVolume != null &&
            globalVolume.profile.TryGet(out ColorAdjustments adjustments))
        {
            colorAdjustments = adjustments;
        }
        else
        {
            Debug.LogWarning("Color Adjustments could not be found.");
        }
    }

    private void Start()
    {
        UpdateVisualsInstantly();
    }

    public void UpdateVisuals()
    {
        if (colorAdjustments == null)
            return;

        float targetSaturation = GetTargetSaturation();

        if (saturationCoroutine != null)
        {
            StopCoroutine(saturationCoroutine);
        }

        saturationCoroutine = StartCoroutine(ChangeSaturation(targetSaturation));
    }

    private IEnumerator ChangeSaturation(float targetSaturation)
    {
        float startSaturation = colorAdjustments.saturation.value;

        float elapsed = 0f;

        while (elapsed < visualTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / visualTransitionDuration;

            colorAdjustments.saturation.value =
                Mathf.Lerp(
                    startSaturation,
                    targetSaturation,
                    progress
                );

            yield return null;
        }

        colorAdjustments.saturation.value = targetSaturation;
        saturationCoroutine = null;
    }

    private float GetTargetSaturation()
    {
        if (timeTravelController.CurrentTimeline == Timeline.Present)
        {
            return 0f;
        }

        float dayProgress = 1f - ((float)dayManager.DaysRemaining / dayManager.MaximumDays);

        return Mathf.Lerp(-100f, 0f, dayProgress);
    }

    private void UpdateVisualsInstantly()
    {
        if (colorAdjustments == null)
            return;

        colorAdjustments.saturation.value = GetTargetSaturation();
    }
}