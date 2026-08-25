using UnityEngine;
using UnityEngine.UI;

public class OrganisationBarUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

    void OnEnable()
    {
        if (OrganisationManager.Instance != null)
        {
            OrganisationManager.Instance.OnProgressChanged.AddListener(UpdateBar);
            UpdateBar(OrganisationManager.Instance.PercentOrganised);
        }
    }

    void OnDisable()
    {
        if (OrganisationManager.Instance != null)
        {
            OrganisationManager.Instance.OnProgressChanged.RemoveListener(UpdateBar);
        }
    }

    private void UpdateBar(float percent)
    {
        if (progressSlider != null)
        {
            progressSlider.value = percent;
        }
    }
}
