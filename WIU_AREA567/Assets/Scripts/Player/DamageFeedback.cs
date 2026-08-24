using System.Collections;
using UnityEngine;

// Generic hurt/death/HP feedback - hook these methods to a Damageable's
// UnityEvents in the Inspector. Works on Player, TestAlien, or any future
// enemy - just needs a SpriteRenderer. Placeholder visuals (color flash,
// disable on death) until real hurt/death sprites and VFX exist.
[RequireComponent(typeof(SpriteRenderer))]
public class DamageFeedback : MonoBehaviour
{
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Bind to Damageable's OnDamaged (int) - Dynamic Int32 section in the dropdown.
    public void OnHurt(int amount)
    {
        Debug.Log($"{gameObject.name} took {amount} damage");

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    // Bind to Damageable's OnDeath ()
    public void OnDeath()
    {
        Debug.Log($"{gameObject.name} died");
        gameObject.SetActive(false); // placeholder - swap for real death animation/VFX later
    }

    // Bind to Damageable's OnHealthChanged (int, int) - Dynamic Int32, Int32 section.
    public void OnHealthChanged(int current, int max)
    {
        Debug.Log($"{gameObject.name} HP: {current}/{max}");
    }
}