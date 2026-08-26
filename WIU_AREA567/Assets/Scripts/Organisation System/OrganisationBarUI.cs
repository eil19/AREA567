using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrganisationBarUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        if (OrganisationManager.Instance != null)
        {
            OrganisationManager.Instance
                .OnProgressChanged
                .AddListener(UpdateBar);

            UpdateBar(
                OrganisationManager.Instance
                    .PercentOrganised
            );
        }
    }

    private void OnDisable()
    {
        if (OrganisationManager.Instance != null)
        {
            OrganisationManager.Instance
                .OnProgressChanged
                .RemoveListener(UpdateBar);
        }
    }

    private void UpdateBar(float percent)
    {
        if (progressSlider != null)
            progressSlider.value = percent;

        if (progressText != null)
        {
            progressText.text =
                Mathf.RoundToInt(percent * 100f) +
                "% Restored";
        }

        if (statusText != null)
        {
            statusText.text =
                OrganisationManager.Instance != null &&
                OrganisationManager.Instance.HasMetThreshold
                    ? "Time Travel Available"
                    : "Restore the laboratory";
        }
    }
}