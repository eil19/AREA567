using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged; // (current, max) - hook up to HUD
    public UnityEvent<int> OnDamaged; // (amount taken) - fires on any non-fatal hit, for hurt-flash animations
    public UnityEvent OnDeath;

    [Header("Audio (optional)")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    [Header("VFX (optional)")]
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private GameObject deathVFXPrefab;

    // add an AlienShield component to). Absorbs damage before health does.
    private AlienShield shield;

    void Awake()
    {
        currentHealth = maxHealth;
        shield = GetComponent<AlienShield>();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        if (shield != null && shield.HasShield)
        {
            amount = shield.AbsorbDamage(amount);
            if (amount <= 0) return; // shield absorbed the whole hit - health untouched
        }

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (hitVFXPrefab != null) Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
            // AudioManager.Instance?.PlaySFX(deathClip); // TODO: re-enable once AudioManager exists in this project
            if (deathVFXPrefab != null) Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            OnDeath?.Invoke();
        }
        else
        {
            // AudioManager.Instance?.PlaySFX(hurtClip); // TODO: re-enable once AudioManager exists in this project
            OnDamaged?.Invoke(amount);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Forces death regardless of current health - use for instant-kill
    // hazards (traps, environmental damage, etc.)
    public void Kill()
    {
        if (currentHealth <= 0) return;
        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        // AudioManager.Instance?.PlaySFX(deathClip); // TODO: re-enable once AudioManager exists in this project
        if (deathVFXPrefab != null) Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        OnDeath?.Invoke();
    }
}