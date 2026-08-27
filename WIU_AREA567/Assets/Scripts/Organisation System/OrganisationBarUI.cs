using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrganisationBarUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text statusText;

    private OrganisationManager organisationManager;

    private void Start()
    {
        organisationManager =
            OrganisationManager.Instance;

        if (organisationManager == null)
        {
            organisationManager =
                FindFirstObjectByType<
                    OrganisationManager>();
        }

        if (organisationManager == null)
        {
            return;
        }

        organisationManager.OnProgressChanged
            .AddListener(UpdateBar);

        UpdateBar(
            organisationManager.PercentOrganised
        );
    }

    private void OnDestroy()
    {
        if (organisationManager != null)
        {
            organisationManager.OnProgressChanged
                .RemoveListener(UpdateBar);
        }
    }

    private void UpdateBar(float percent)
    {
        if (progressSlider != null)
        {
            progressSlider.value = percent;
        }

        if (progressText != null)
        {
            progressText.text =
                Mathf.RoundToInt(
                    percent * 100f
                ) + "%";
        }

        if (statusText != null)
        {
            statusText.text =
                organisationManager != null &&
                organisationManager.HasMetThreshold
                    ? "Time Travel Available"
                    : "Restore the laboratory";
        }
    }
}