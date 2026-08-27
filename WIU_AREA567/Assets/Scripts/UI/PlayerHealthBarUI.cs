using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI :
    MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Damageable playerHealth;

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
        }

        if (playerHealth == null)
        {
            PlayerController player =
                FindFirstObjectByType<
                    PlayerController>();

            if (player != null)
            {
                playerHealth =
                    player.GetComponent<
                        Damageable>();
            }
        }

        if (playerHealth == null)
        {
            Debug.LogWarning(
                "PlayerHealthBarUI could not find Player Damageable."
            );

            return;
        }

        playerHealth.OnHealthChanged
            .AddListener(UpdateHealthBar);

        UpdateHealthBar(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged
                .RemoveListener(
                    UpdateHealthBar
                );
        }
    }

    public void UpdateHealthBar(
        int currentHealth,
        int maxHealth)
    {
        if (healthSlider == null)
            return;

        healthSlider.value =
            maxHealth <= 0
                ? 0f
                : Mathf.Clamp01(
                    (float)currentHealth /
                    maxHealth
                );
    }
}