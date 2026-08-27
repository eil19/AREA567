using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Damageable playerHealth;

    private void Start()
    {
        if (playerHealth == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerHealth = player.GetComponent<Damageable>();
            }
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealthBarUI could not find the Player Damageable component.", this);
            return;
        }

        playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
        UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthSlider.value = maxHealth <= 0
            ? 0f
            : (float)currentHealth / maxHealth;
    }
}
