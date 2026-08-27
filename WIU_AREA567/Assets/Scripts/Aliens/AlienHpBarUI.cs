using UnityEngine;
using UnityEngine.UI;

public class AlienHpBarUI : MonoBehaviour
{
    [SerializeField] private Damageable damageable;
    [SerializeField] private Slider healthSlider;

    [Header("Visibility")]
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private GameObject visualRoot; // what actually gets shown/hidden - defaults to this GameObject if left empty

    void Awake()
    {
        if (damageable == null)
        {
            damageable = GetComponentInParent<Damageable>();
        }
    }

    void OnEnable()
    {
        if (damageable == null) return;

        damageable.OnHealthChanged.AddListener(UpdateBar);
        damageable.OnDeath.AddListener(HandleDeath);
    }

    void Start()
    {
        if (damageable != null)
        {
            UpdateBar(damageable.CurrentHealth, damageable.MaxHealth);
        }
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