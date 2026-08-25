using UnityEngine;
using UnityEngine.UI;

public class OrganisationBarUI : MonoBehaviour
{
    //organisation progress. Retries subscribing every frame until OrganisationManager.Instance actually exists, 
    [SerializeField] private Slider progressSlider;

    private bool subscribed;

    void OnEnable()
    {
        TrySubscribe();
    }

    void Update()
    {
        if (!subscribed)
        {
            TrySubscribe();
        }
    }

    void OnDisable()
    {
        if (subscribed && OrganisationManager.Instance != null)
        {
            OrganisationManager.Instance.OnProgressChanged.RemoveListener(UpdateBar);
        }
        subscribed = false;
    }

    private void TrySubscribe()
    {
        if (OrganisationManager.Instance == null) return;

        OrganisationManager.Instance.OnProgressChanged.AddListener(UpdateBar);
        UpdateBar(OrganisationManager.Instance.PercentOrganised);
        subscribed = true;
    }

    private void UpdateBar(float percent)
    {
        if (progressSlider != null)
        {
            progressSlider.value = percent;
        }
    }
}
