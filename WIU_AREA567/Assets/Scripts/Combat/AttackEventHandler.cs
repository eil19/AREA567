using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AttackEventHandler : MonoBehaviour
{
    private Transform attackPoint;
    private Transform rangedSpawnPoint;
    private Animator animator;
    private PlayerController playerController;

    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float attackRadius = 0.2f;
    [SerializeField] private int damageAmount = 10;

    [Header("Ranged")]
    [Tooltip("Projectile spawned when the player has weapon 2 (ranged) equipped.")]
    [SerializeField] private GameObject rangedProjectilePrefab;
    [SerializeField, Min(0f)] private float rangedFireCooldown = 0.25f;
    private float nextRangedFireTime;

    [Header("Taser")]
    [Tooltip("GameObject tag used to identify Aliens - Taser only affects objects with this tag.")]
    [SerializeField] private string alienTag = "Alien";
    [SerializeField] private float stunDuration = 3f;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        attackPoint = playerController.AttackPoint;
        rangedSpawnPoint = playerController.RangedSpawnPoint;
        animator = GetComponent<Animator>();
        if (attackPoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: AttackEventHandler couldn't find an Attack Point - assign one on PlayerController's Inspector field.");
        }
    }

    [ContextMenu("Test Taser (Editor Only)")]
    private void DebugTestTaser()
    {
        TaserCheck();
    }

    public void AttackCheck()
    {
        if (attackPoint == null) return;

        attackPoint.gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, layerToCheck);
        if (hit != null && hit.TryGetComponent(out Damageable damagedObject))
        {
            damagedObject.TakeDamage(damageAmount);
        }
    }

    public void TaserCheck()
    {
        if (attackPoint == null) return;

        attackPoint.gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, layerToCheck);
        if (hit != null && hit.CompareTag(alienTag) && hit.TryGetComponent(out Stunnable stunnableTarget))
        {
            stunnableTarget.Stun(stunDuration);
        }
    }

    // Called by PlayerController when the Attack input is pressed with weapon 2 (Ranged) equipped.
    // Unlike melee/taser this fires immediately, so it does not require an animation event.
    public bool FireRangedAttack()
    {
        if (rangedProjectilePrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}: No ranged projectile prefab is assigned.");
            return false;
        }

        if (Time.time < nextRangedFireTime) return false;

        Vector2 direction = playerController.FacingDirection;
        Vector3 spawnPosition = rangedSpawnPoint != null
            ? rangedSpawnPoint.position
            : attackPoint != null ? attackPoint.position : transform.position;
        GameObject projectile = Instantiate(rangedProjectilePrefab, spawnPosition, Quaternion.identity);

        if (!projectile.TryGetComponent(out PlayerProjectile playerProjectile))
        {
            Debug.LogError($"{rangedProjectilePrefab.name} must have a PlayerProjectile component.");
            Destroy(projectile);
            return false;
        }

        playerProjectile.Launch(direction);
        nextRangedFireTime = Time.time + rangedFireCooldown;
        return true;
    }

    public void AttackEnd()
    {
        if (attackPoint == null) return;
        attackPoint.gameObject.SetActive(false);
        if (animator != null) animator.SetBool("IsBusy", false);
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StartCoroutine(BoostRoutine(multiplier, duration));
    }

    private IEnumerator BoostRoutine(float multiplier, float duration)
    {
        int original = damageAmount;
        damageAmount = Mathf.RoundToInt(damageAmount * multiplier);
        yield return new WaitForSeconds(duration);
        damageAmount = original;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}