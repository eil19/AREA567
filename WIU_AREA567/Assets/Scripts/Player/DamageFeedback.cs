using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageFeedback : MonoBehaviour
{
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    private Animator animator;
    private PlayerController playerController;
    private AttackEventHandler attackEventHandler;
    private PlayerInteractor playerInteractor;
    private Rigidbody2D body;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        attackEventHandler = GetComponent<AttackEventHandler>();
        playerInteractor = GetComponent<PlayerInteractor>();
        body = GetComponent<Rigidbody2D>();
    }

    public void OnHurt(int amount)
    {
        Debug.Log($"{gameObject.name} took {amount} damage");

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
        // No animation trigger - just the color flash, per design choice.
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    public void OnDeath()
    {
        Debug.Log($"{gameObject.name} died");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDeathAnimationComplete()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
        if (playerController != null) playerController.enabled = false;
        if (attackEventHandler != null) attackEventHandler.enabled = false;
        if (playerInteractor != null) playerInteractor.enabled = false;

        Debug.Log($"{gameObject.name} is now fully dead - controls disabled.");
    }

    public void OnHealthChanged(int current, int max)
    {
        Debug.Log($"{gameObject.name} HP: {current}/{max}");
    }
}