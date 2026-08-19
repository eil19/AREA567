using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    
    [Header("Health")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }
    public bool IsDead { get; private set; }

    public UnityEvent onHealthChanged;
    public UnityEvent onDeath;


    public event Action<int> OnDamaged;
    public event Action<int> OnHealed;
    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke();
    }

    public virtual void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0) return;

        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Fired after currentHealth is updated
        NotifyDamaged(damage);

        onHealthChanged?.Invoke();

        if (currentHealth == 0)
        {
            IsDead = true;
            OnDeath?.Invoke();
            onDeath?.Invoke();
        }
    }


    protected void NotifyDamaged(int damage)
    {
        OnDamaged?.Invoke(damage);
    }


    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (IsDead || damage <= 0) return;

        if (knockbackForce > 0f && TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        TakeDamage(damage);
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnHealed?.Invoke(amount);
        onHealthChanged?.Invoke();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        IsDead = false;
        onHealthChanged?.Invoke();
    }
}