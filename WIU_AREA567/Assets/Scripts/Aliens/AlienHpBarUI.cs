using UnityEngine;
using UnityEngine.UI;

public class AlienHpBarUI : MonoBehaviour
{
    [SerializeField] private Damageable damageable;
    [SerializeField] private Slider healthSlider;

    [Header("Visibility")]
    [Tooltip("Hide the bar entirely while health is full - common for regular aliens so undamaged enemies don't show a bar at all.")]
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private GameObject visualRoot; // what actually gets shown/hidden - defaults to this GameObject if left empty

    void Awake()
    {
        if (damageable == null)
        {
            damageable = GetComponentInParent<Damageable>();
        }

        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }
    }

    void OnEnable()
    {
        if (damageable == null) return;

        damageable.OnHealthChanged.AddListener(UpdateBar);
        damageable.OnDeath.AddListener(HandleDeath);

        // Reflect current health immediately instead of waiting for the next hit.
        UpdateBar(damageable.CurrentHealth, damageable.MaxHealth);
    }

    void OnDisable()
    {
        if (damageable == null) return;

        damageable.OnHealthChanged.RemoveListener(UpdateBar);
        damageable.OnDeath.RemoveListener(HandleDeath);
    }

    private void UpdateBar(int current, int max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }

        if (hideWhenFull && visualRoot != null)
        {
            visualRoot.SetActive(current < max);
        }
    }

    private void HandleDeath()
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }
    }
}
