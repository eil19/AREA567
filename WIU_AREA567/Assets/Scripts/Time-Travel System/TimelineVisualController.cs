using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimelineVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;

    [Header("This Scene")]
    [SerializeField] private Timeline sceneTimeline;

    [Header("Saturation")]
    [SerializeField] private float pastSaturation = -65f;
    [SerializeField] private float presentDay7Saturation = -35f;
    [SerializeField] private float presentDay0Saturation = 0f;

    [Header("Transition")]
    [SerializeField] private float visualTransitionDuration = 0.4f;

    private DayManager dayManager;
    private ColorAdjustments colorAdjustments;
    private Coroutine saturationCoroutine;

    private void Awake()
    {
        if (globalVolume != null &&
            globalVolume.profile.TryGet(
                out ColorAdjustments adjustments))
        {
            colorAdjustments = adjustments;
        }
        else
        {
            Debug.LogWarning(
                "TimelineVisualController: Color Adjustments not found."
            );
        }
    }

    private void Start()
    {
        dayManager =
            FindFirstObjectByType<DayManager>();

        if (dayManager == null)
        {
            Debug.LogWarning(
                "TimelineVisualController: DayManager not found."
            );

            return;
        }

        dayManager.OnDayChanged.AddListener(
            UpdateVisuals
        );

        UpdateVisualsInstantly();
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnDayChanged.RemoveListener(
                UpdateVisuals
            );
        }
    }

    public void UpdateVisuals(int daysRemaining)
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (colorAdjustments == null ||
            dayManager == null)
        {
            return;
        }

        float target =
            GetTargetSaturation();

        if (saturationCoroutine != null)
        {
            StopCoroutine(
                saturationCoroutine
            );
        }

        saturationCoroutine =
            StartCoroutine(
                ChangeSaturation(target)
            );
    }

    private float GetTargetSaturation()
    {
        // Past is ALWAYS desaturated.
        if (sceneTimeline == Timeline.Past)
        {
            return pastSaturation;
        }

        // Present slowly regains colour.
        float progress =
            1f -
            ((float)dayManager.DaysRemaining /
             dayManager.MaximumDays);

        return Mathf.Lerp(
            presentDay7Saturation,
            presentDay0Saturation,
            progress
        );
    }

    private IEnumerator ChangeSaturation(
        float targetSaturation)
    {
        float start =
            colorAdjustments.saturation.value;

        float elapsed = 0f;

        while (elapsed <
               visualTransitionDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed /
                    visualTransitionDuration
                );

            colorAdjustments.saturation.value =
                Mathf.Lerp(
                    start,
                    targetSaturation,
                    progress
                );

            yield return null;
        }

        colorAdjustments.saturation.value =
            targetSaturation;

        saturationCoroutine = null;
    }

    private void UpdateVisualsInstantly()
    {
        if (colorAdjustments == null ||
            dayManager == null)
        {
            return;
        }

        colorAdjustments.saturation.value =
            GetTargetSaturation();
    }
}